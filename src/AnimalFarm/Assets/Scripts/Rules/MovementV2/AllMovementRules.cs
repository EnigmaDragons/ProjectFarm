using System;
using System.Collections.Generic;
using System.Linq;

public static class AllMovementRules
{
    public static Permissible Can(this LevelStateSnapshot l, MovementType t, TilePoint from, TilePoint to)
    {
        if (t == MovementType.Eat || t == MovementType.Enter || t == MovementType.SwimRide || t == MovementType.Activate)
            return AdjacentTargetPieceMove(l, t, from, to);
        if (t == MovementType.Jump)
            return CanJump(l, from, to);

#if UNITY_EDITOR
        throw new NotImplementedException("Unhandled Movement Type");
#endif
        return new Permissible($"Unhandled Movement Type: {t}");
    }

    private static Permissible CanJump(this LevelStateSnapshot l, TilePoint from, TilePoint to)
    {
        var movementType = MovementType.Jump;
        if (!l.Pieces.TryGetValue(from, out var piece))
            return new Permissible($"No Piece at Tile {from}");
        var canPerformMove = piece.Can(movementType);
        if (!canPerformMove)
            return new Permissible($"Movement Type {movementType} not permitted for a {piece} piece");

        if (!to.IsInBounds(l.Size))
            return new Permissible($"Target Tile is out of level map bounds: {to}. (Reported map size is {l.Size})");
        if (to.DistanceFrom(from) > 2)
            return new Permissible("Only jumps of distance 2 currently allowed.");
        if (!l.Floors.ContainsKey(to))
            return new Permissible($"Target Tile {to} does not have a ground to land on.");
        if (!l.Floors[to].Rules().IsWalkable)
            return new Permissible($"Target Tile {to} cannot be landed on.");
        if (l.Pieces.TryGetValue(to, out var blocker) && blocker.Rules().IsBlocking)
            return new Permissible($"Target Tile {to} is currently blocked by {blocker}");
        var inBetweens = to.InBetween(from);
        foreach (var inBetween in inBetweens)
            if (!l.Pieces.TryGetValue(inBetween, out var maybeJumpable) || !maybeJumpable.Rules().IsJumpable)
                return new Permissible($"{inBetween} {piece} is not Jumpable");

        return Permissible.Allowed();
    }
    
    private static Permissible AdjacentTargetPieceMove(this LevelStateSnapshot l, MovementType movementType, TilePoint from, TilePoint to)
    {
        if (!l.Pieces.TryGetValue(from, out var piece))
            return new Permissible($"No Piece at Tile {from}");
        var canPerformMove = piece.Can(movementType);
        if (!canPerformMove)
            return new Permissible($"Movement Type {movementType} not permitted for a {piece} piece");
        
        if (!l.Pieces.TryGetValue(to, out var targetPiece))
            return new Permissible($"No Piece at Tile {to}");

        var issues = new List<string>();
        var targetWorksForMoveType = targetPiece.Rules().MovementTargetTypes.Any(mt => mt == movementType);
        if (!targetWorksForMoveType)
            issues.Add($"{targetPiece} is not {movementType}-able");
        var isOneAway = from.IsAdjacentTo(to) && from.DistanceFrom(to) == 1;
        if (!isOneAway)
            issues.Add($"Distance only allowed to be 1 for {movementType}");
        return new Permissible(issues.ToArray());
    }
}
