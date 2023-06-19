using TMPro;
using UnityEngine;

public class DrawDevHelpers : MonoBehaviour
{
    [SerializeField] private GameObject prototype;
    [SerializeField] private GameObject parent;
    [SerializeField] private BoolReference show = new BoolReference(true);
    [SerializeField] private int min = 0;
    [SerializeField] private int max = 10;

    private void Start()
    {
        for(var x = min; x < max; x++)
            for (var y = min; y < max; y++)
            { 
                var go = Instantiate(prototype, new Vector3(x, 0, y), Quaternion.identity, parent.transform);
                go.GetComponentInChildren<TextMeshPro>().text = $"{x},{y}";
            }
    }

    private void Update()
    {
        if (show.Value != parent.gameObject.activeSelf)
            parent.gameObject.SetActive(show.Value);
    }
}
