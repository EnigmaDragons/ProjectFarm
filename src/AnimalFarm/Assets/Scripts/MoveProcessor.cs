using System.Linq;
using UnityEngine;

public sealed class MoveProcessor : OnMessage<MoveToRequested, LevelReset, UndoPieceMoved>
{
    [SerializeField] private CurrentLevelMap map;

    private int _moveNumber = 0;
    private bool _debugLoggingEnabled;
    
    protected override void Execute(MoveToRequested m)
    {        
        var proposal = MovementIsPossible(map, m);
        if (proposal.IsPresent)
            Message.Publish(new PieceMoved(proposal.Value.Type, proposal.Value.Piece, m.From, m.To, _moveNumber++));
    }

    public static Maybe<MovementProposed> MovementIsPossible(CurrentLevelMap map, MoveToRequested m)
    {
        if (m.Piece.GetComponent<MovementEnabled>() == null)
            return Maybe<MovementProposed>.Missing();

        var movementProposals = map.MovementOptionRules
            .Where(r => m.Piece.GetComponent<MovementEnabled>().Types.Any(t => r.Type == t))
            .Where(x => x.IsPossible(m))
            .Select(x => new MovementProposed(x.Type, m.Piece, m.From, m.To)).ToList();

        Debug.Log("Possible Moves: " + string.Join(", ", movementProposals.Select(mp => $"{mp.Type} - {mp.From} -> {mp.To}").ToArray()));
        foreach (var proposal in movementProposals)
        {
            var notValidReasons = map.MovementRestrictions.Select(r => (r.name, r.IsValid(proposal)));
            if (!notValidReasons.None())
                return proposal;
            Debug.Log("Not Valid Reasons: " + string.Join(", ", notValidReasons.Where(x => !x.Item2).Select(x => x.name).ToArray()));
        }

        return Maybe<MovementProposed>.Missing();
    }

    protected override void Execute(LevelReset msg) => _moveNumber = 0;
    protected override void Execute(UndoPieceMoved msg) => _moveNumber = msg.MoveNumber - 1;
}
