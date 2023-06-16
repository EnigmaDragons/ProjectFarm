using UnityEngine;

public class RandomizeShader : MonoBehaviour
{
    [SerializeField] private Renderer renderer;
    [SerializeField] private float randomMultiplier;

    private void Start()
    {
        renderer.material.SetFloat("_Random", Random.value * randomMultiplier);
    }
}
