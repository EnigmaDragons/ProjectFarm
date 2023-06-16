using TMPro;
using UnityEngine;

public class TileLeaveCounterPresenter : OnMessage<PieceMoved>
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private BoolReference isEnabled;

    private int _count = 0;
    private TilePoint _pos;

    private void Awake()
    {
        _pos = new TilePoint(gameObject);
        if (Debug.isDebugBuild && isEnabled) return;
        
        text.enabled = false;
        enabled = false;
    }

    protected override void Execute(PieceMoved msg)
    {
        if (!msg.From.Equals(_pos)) return;
        
        _count++;
        text.text = _count.ToString();
    }
}
