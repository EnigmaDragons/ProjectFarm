using UnityEngine;

public class StartWithRandomRotation : MonoBehaviour
{
    [SerializeField] private Vector3 multiplier;

    private void Start()
    {
        var eulerAngles = transform.localRotation.eulerAngles;
        var newEulerAngles = new Vector3(eulerAngles.x + multiplier.x * Random.value, eulerAngles.y + multiplier.y * Random.value, eulerAngles.z + multiplier.z * Random.value);
        transform.localRotation = Quaternion.Euler(newEulerAngles);
    }
}
