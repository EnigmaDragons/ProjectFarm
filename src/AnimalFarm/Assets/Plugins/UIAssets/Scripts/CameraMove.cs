using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour
{
    public float speed = 0.1F;
    private Vector3 clampedPos;
    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            transform.Translate(0, -touchDeltaPosition.y * speed, 0);
        }
        if (transform.position.y >= 20)
        {
            transform.position = new Vector3 (0,20,0);
        }
        if (transform.position.y <= 0)
        {
            transform.position = new Vector3(0, 0, 0);
        }
    }
}