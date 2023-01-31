using UnityEngine;
using UnityEngine.Rendering;

public class VolumeScript : MonoBehaviour
{

    [SerializeField] private Volume volume;

    public void StartChromatik(float x)
    {
            volume.weight = x;
    }
}