using System.Linq;
using UnityEngine;

public class OnSelectedShowUserInteractionVisuals : MonoBehaviour
{
    [SerializeField] private GameObject[] options;
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private MovementEnabled movement;
    [SerializeField] private BoolVariable gameInputActive;
    [SerializeField] private BoolVariable showMovementOptions;

    private bool _selected;
    private bool _gameInputActive;

    private void OnEnable()
    {
        Message.Subscribe<PieceSelected>(x => Selected(x.Piece), this);
        Message.Subscribe<PieceDeselected>(x => Deselected(), this);
    }

    private void OnDisable() => Message.Unsubscribe(this);

    private void Update()
    {
        if (gameInputActive.Value != _gameInputActive)
        {
            _gameInputActive = gameInputActive.Value;
            if (_gameInputActive && _selected)
                Selected(gameObject);
            else
                options.ForEach(x => x.SetActive(false));
        }

    }

    private void Selected(GameObject piece)
    {
        if (piece == gameObject)
        {
            _selected = true;
            options.ForEach(ProcessOption);
        }
        else
            Deselected();
    }

    private void Deselected()
    {
        _selected = false;
        options.ForEach(x => x.SetActive(false));
    }

    private void ProcessOption(GameObject option)
    {
        if (!showMovementOptions.Value)
        {
            option.SetActive(false);
            return;
        }
    
        var from = new TilePoint(gameObject);
        var to = new TilePoint(
            (int) (gameObject.transform.localPosition.x + option.transform.localPosition.x),
            (int) (gameObject.transform.localPosition.y + option.transform.localPosition.y));
        var movementProposals = map.MovementOptionRules
            .Where(r => movement.Types.Any(t => r.Type == t))
            .Where(x => x.IsPossible(new MoveToRequested(gameObject, from, to)))
            .Select(x => new MovementProposed(x.Type, gameObject, from, to)).ToList();
        option.SetActive(movementProposals.Any(proposal => map.MovementRestrictionRules.All(x => x.IsValid(proposal))));
    }
}
