using System.Linq;
using UnityEngine;

public class DynamicPettingAnimal : OnMessage<ReadyForPettingInit>
{
    [Header("Settings")]
    public float petSpeedThreshold = 3.0f;

    [Header("Component References")]
    public Collider collider;
    public Animator animator;
    public Collider[] sweetSpots;
    public AudioSource audioSource;

    // pet state and enum
    [HideInInspector]
    public PetState petState = PetState.notBeingPetted;
    
    protected override void AfterEnable() => Init();

    public void Init()
    {
        collider = GetComponentsInChildren<Collider>().Where(x => x.gameObject.activeInHierarchy).First();
        animator = GetComponentsInChildren<Animator>().Where(x => x.gameObject.activeInHierarchy).First();
        sweetSpots = GetComponentsInChildren<Collider>().Where(x => x.gameObject.activeInHierarchy && x.transform.tag.Equals("SweetSpot")).ToArray();
        audioSource = GetComponentsInChildren<AudioSource>().Where(x => x.gameObject.activeInHierarchy).First();
        if (audioSource != null)
            Message.Publish(new AnimalAudioSourceChanged(audioSource));
    }

    protected override void Execute(ReadyForPettingInit msg) => Init();
}
