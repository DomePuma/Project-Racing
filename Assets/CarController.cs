using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using TMPro.EditorUtilities;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    PlayerController _controls;

    private Vector2 _moveDirection;

    private PlayerInput _inputs;

    public float _maxSpeed;
    private float _currentSpeed;
    private float _currentAcceleration;

    public float _maxRotationSpeed = 1;


    public CinemachineBrain _brain;
    public CinemachineVirtualCamera _camera1;
    public CinemachineVirtualCamera _camera2;

    // Start is called before the first frame update
    void Start()
    {
        _inputs = GetComponent<PlayerInput>();

        InputAction move = _inputs.actions["Move"];
        InputAction changeCamera = _inputs.actions["ChangeCamera"];

        move.started += OnMoveStarted;
        move.performed += OnMovePerformed;
        move.canceled += OnMoveCanceled;

        changeCamera.performed += OnChangeCamera;
    }

    private void OnMoveCanceled(InputAction.CallbackContext obj)
    {
        _moveDirection = Vector2.zero;
    }

    public void OnMovePerformed(InputAction.CallbackContext context)
    {
        Debug.Log($"Move performed : {context.ReadValue<Vector2>()}");

        _moveDirection = context.ReadValue<Vector2>();
    }

    private void OnMoveStarted(InputAction.CallbackContext context)
    {
        Debug.Log($"Move started : {context.ReadValue<Vector2>()}");

        _moveDirection = context.ReadValue<Vector2>();
    }
    void OnChangeCamera(InputAction.CallbackContext context)
    {
        var currentCamera = _brain.ActiveVirtualCamera as CinemachineVirtualCamera;

        if(currentCamera == _camera1)
        {
            _camera1.Priority = 0;
            _camera2.Priority = 10;
        }
        else
        {
            _camera1.Priority = 10;
            _camera2.Priority = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // R?cup?re les donn?es de mouvement
        float rotationAngle = _moveDirection.x;
        float acceleration = _moveDirection.y;

        // Si on acc?l?re pas (lach?)
        if (acceleration == 0)
        {
            _currentAcceleration = Mathf.Lerp(_currentAcceleration, 0, Time.deltaTime);
        }
        else if (acceleration < 0)
        {
            //Si on est en train d'avancer, on freine
            if (_currentAcceleration > 0)
            {
                _currentAcceleration -= Time.deltaTime;
            }
            else // On recule
            {
                _currentAcceleration += acceleration * Time.deltaTime;
            }
        }
        else
        {
            // On acc?l?re progressivement
            _currentAcceleration += acceleration * Time.deltaTime;
        }

        _currentAcceleration = Mathf.Clamp(_currentAcceleration, -1, 1);

        if (_currentAcceleration >= 0)
        {
            _currentSpeed = Mathf.Lerp(0, _maxSpeed, _currentAcceleration);
        }
        else
        {
            _currentSpeed = Mathf.Lerp(0, -_maxSpeed, -_currentAcceleration);
        }

        // Influence accelerations sur la rotation
        rotationAngle = rotationAngle * _currentAcceleration * _maxRotationSpeed * Time.deltaTime;

        transform.Rotate(0, rotationAngle, 0);
        transform.position = transform.position +
                             transform.forward * (_currentSpeed * Time.deltaTime);

    }
}
