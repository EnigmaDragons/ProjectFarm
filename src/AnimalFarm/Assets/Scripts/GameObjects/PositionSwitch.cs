using UnityEngine;

public class PositionSwitch : MonoBehaviour
{
    [SerializeField] private LayoutMode layout;
    [SerializeField] private Vector3 tallPosition;
    [SerializeField] private Vector3 tallRotation; 
    [SerializeField] private Vector3 widePosition;
    [SerializeField] private Vector3 wideRotation;
    [SerializeField] private bool effectsRotation;
    [SerializeField] private bool effectsX;
    [SerializeField] private bool effectsY;
    [SerializeField] private bool effectsZ;

    private Transform t;
    
    private void Awake()
    {
        t = transform;
    }
    
    private void Update()
    {
        if (effectsRotation)
            t.localEulerAngles = layout.IsTall ? tallRotation : wideRotation;

        if (!effectsX && !effectsY && !effectsZ)
            return;
        
        var pos = t.localPosition;
        t.localPosition = new Vector3(effectsX ? (layout.IsTall ? tallPosition.x : widePosition.x) : pos.x,
            effectsY ? (layout.IsTall ? tallPosition.y : widePosition.y) : pos.y,
            effectsZ ? (layout.IsTall ? tallPosition.z : widePosition.z) : pos.z);
    }
}
