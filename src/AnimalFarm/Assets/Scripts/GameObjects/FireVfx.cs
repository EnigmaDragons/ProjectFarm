using UnityEngine;

public class FireVfx : MonoBehaviour
{
    [SerializeField] private GameObject particles;
    
    public void Activate()
    {
        particles.SetActive(true);
    }

    public void Deactivate()
    {
        particles.SetActive(false);
    }
}
