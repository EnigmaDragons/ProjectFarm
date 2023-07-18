using UnityEngine;

public class ParticlesOnAnimalSelected : OnMessage<HeroAnimalSelected>
{
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private GameObject target;
    
    protected override void Execute(HeroAnimalSelected msg)
    {
        if (msg.Selected == HeroAnimal.NotSelected)
        {
            target.SetActive(false);
            return;
        }
        
        target.SetActive(true);
        particle.Stop();
        particle.Play();
    }
}
