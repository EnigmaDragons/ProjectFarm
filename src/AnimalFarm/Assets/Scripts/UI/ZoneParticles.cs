using UnityEngine;

public class ZoneParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private CurrentZone zone;

    public void Start()
    {
        var main = particleSystem.main;
        main.startColor = zone.Zone.BackgroundColor;
    }
}
