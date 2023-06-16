using UnityEngine;

public sealed class DeliveryTimeTutorial : OnMessage<PieceMoved>
{
    [SerializeField] private DeliveryTimeTutorialSegment[] segments;
    [SerializeField] private bool beginImmediately = true;

    private int _index = 0;

    private void Start()
    {
        if (beginImmediately)
            Advance();
    }
    
    protected override void Execute(PieceMoved msg)
    {
        if (_index >= segments.Length || !msg.To.Equals(segments[_index].Location))
            return;

        Advance();
    }

    private void Advance()
    {
        segments[_index].Execute();
        _index++;
    }
}
