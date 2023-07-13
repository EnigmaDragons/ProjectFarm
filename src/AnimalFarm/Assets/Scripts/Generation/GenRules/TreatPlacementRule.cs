using System.Linq;
using UnityEngine;

public class TreatPlacementRule : MapPieceGenRule
{
    private bool CanPlace(GenContextData ctx) => !ctx.Pieces.Any(x => x.Value == MapPiece.Treat);
    
    public override int Priority => 1;
    public override MapPiece Piece => MapPiece.Treat;
    public override bool MustPlace(GenContextData ctx) => ctx.MaxRemainingMoves == 1 && CanPlace(ctx);
    public override bool ShouldPlace(GenContextData ctx) => CanPlace(ctx) && Rng.Dbl() < Mathf.Clamp(1f / ctx.MaxRemainingMoves, 0f, 1f);
    
    public override void Apply(GenWipData data) => FoodPlacementRule.Apply(data, Piece);
}
