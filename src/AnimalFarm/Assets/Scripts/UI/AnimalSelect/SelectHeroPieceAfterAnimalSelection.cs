using UnityEngine;

public class SelectHeroPieceAfterAnimalSelection : OnMessage<AnimalSelectionBegun, AnimalSelectionFinished>
{
    [SerializeField] private CurrentSelectedPiece currentPiece;
    [SerializeField] private CurrentLevelMap map;

    protected override void Execute(AnimalSelectionBegun msg) => currentPiece.Deselect();

    protected override void Execute(AnimalSelectionFinished msg) => Message.Publish(new PieceSelected(map.Hero));
}
