using System.Linq;

public class DinoFissurePlacementRule : MapPieceGenRule
{
    private bool CanPlace(GenContextData ctx) => ctx.Pieces.Count > 5 && !ctx.Pieces.Any(x => x.Value == Piece);
    
    public override int Priority => 40;
    public override MapPiece Piece => MapPiece.Dino;

    public override bool MustPlace(GenContextData ctx) => ctx.MustInclude.Contains(Piece) && ctx.MaxRemainingMoves <= 3 && CanPlace(ctx);
    public override bool ShouldPlace(GenContextData ctx) => CanPlace(ctx) && Rng.Dbl() < GenFunctions.AdjustOdds(0.25f, Piece, ctx.Pieces.Values.ToHashSet());

    public override void Apply(GenWipData data)
    {
        var from = data.FromTile.Clone();
        var movingPiece = data.Pieces[from];
        var targetPos = from.GetAdjacents().Where(x => x.IsInBounds(data.Level.MaxX, data.Level.MaxY) && !data.Pieces.ContainsKey(x)).ToArray().Random();
        
        // Place Dino
        // TODO: Pick Fissure Direction
        // Shift for Fissure
        
    }
}
