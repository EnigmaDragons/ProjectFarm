using System.Linq;
using Codice.Client.BaseCommands;
using UnityEngine;

public sealed class MoveProcessor : OnMessage<MoveToRequested, LevelReset, UndoPieceMoved>
{
    [SerializeField] private CurrentHeroAnimal currentAnimal;
    [SerializeField] private CurrentLevelMap map;

    private int _moveNumber = 0;
    private bool _debugLoggingEnabled;
    
    protected override void Execute(MoveToRequested m)
    {        
        var proposal = MovementIsPossible(map, m, currentAnimal.Current);
        if (proposal.IsPresent)
            Message.Publish(new PieceMoved(proposal.Value.Type, proposal.Value.Piece, m.From, m.To, _moveNumber++));
    }

    public static Maybe<MovementProposed> MovementIsPossible(CurrentLevelMap map, MoveToRequested m, HeroAnimal hero)
    {
        var pieceType = map.GetObjectPiece(m.From);
        if (pieceType == MapPiece.HeroAnimal && hero == map.GeniusAnimal && m.To.Equals(map.BarnLocation) && m.From.Equals(map.InitialHeroLocation))
            return new Maybe<MovementProposed>(new MovementProposed(MovementType.Genius, m.Piece, m.From, m.To));
        else 
            Log.SInfo(LogScopes.Movement, $"Genius {map.GeniusAnimal}. Hero: {hero}. HOrigin: {map.InitialHeroLocation}. Barn: {map.BarnLocation}. PieceType: {pieceType}. From: {m.From}. To: {m.To}");

        var mps = map.Snapshot.GetPossibleMoves(m.From);
        Log.SInfo(LogScopes.Movement, $"{pieceType} {m.From} - Possible: " + string.Join(", ", mps.Select(mp => $"{mp.Piece} - {mp.MovementType} - {mp.From} -> {mp.To}").ToArray()));
        var matchingMps = mps.Where(p => p.To.Equals(m.To)).ToArray();
        if (matchingMps.Length > 0)
        {
            var mp = matchingMps[0];
            return new Maybe<MovementProposed>(new MovementProposed(mp.MovementType, m.Piece, m.From, m.To));
        }
        
        return Maybe<MovementProposed>.Missing();
    }

    protected override void Execute(LevelReset msg) => _moveNumber = 0;
    protected override void Execute(UndoPieceMoved msg) => _moveNumber = msg.MoveNumber - 1;
}
