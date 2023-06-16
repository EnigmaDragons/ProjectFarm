using UnityEngine;

public class ParticleSystemRotationSwitch : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private LayoutMode layoutMode;
    [SerializeField] private float tallRotation;
    [SerializeField] private float wideRotation;

    private ParticleSystem.MainModule psMain;
    private float tallRotationRadians;
    private float wideRotationRadians;

    private void Start()
    {
        psMain = particleSystem.main;
        tallRotationRadians = tallRotation * Mathf.Deg2Rad;
        wideRotationRadians = wideRotation * Mathf.Deg2Rad;
    } 
    private void Update() => psMain.startRotation = layoutMode.IsTall ? tallRotationRadians : wideRotationRadians;
}
