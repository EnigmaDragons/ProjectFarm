using UnityEngine;

public class InGameCollectedStar : MonoBehaviour
{
    [SerializeField] private CurrentLevelStars stars;
    [SerializeField] private Vector3Reference position1;
    [SerializeField] private Vector3Reference position2;
    [SerializeField] private Vector3Reference position3;
    [SerializeField] private float speed;
    [SerializeField] private Renderer renderer;
    [SerializeField, ReadOnly] private int starNumber;
    
    private Vector3Reference _target;

    private void OnEnable()
    {
        stars.OnChanged.Subscribe(UpdateExistence, this);
        starNumber = stars.Count + 1;
        if (stars.Count == 0)
            _target = position1;
        else if (stars.Count == 1)
            _target = position2;
        else if (stars.Count == 2)
            _target = position3;
        renderer.material.renderQueue = 4000;
    }

    private void OnDisable() => stars.OnChanged.Unsubscribe(this);

    private void UpdateExistence()
    {
        if (starNumber > stars.Count)
            Destroy(gameObject);
    }

    private void Update()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, _target.Value, speed * Time.deltaTime);
    }
}
