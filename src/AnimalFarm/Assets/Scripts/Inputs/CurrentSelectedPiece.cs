using UnityEngine;

[CreateAssetMenu]
public class CurrentSelectedPiece : ScriptableObject
{
    [SerializeField] private Maybe<GameObject> selected = new Maybe<GameObject>();
    [SerializeField] private GameEvent onChange;

    public GameEvent OnChanged => onChange;
    public Maybe<GameObject> Selected => selected;

    public void Select(GameObject obj)
    {
        selected = obj;
        onChange.Publish();
    }

    public void Deselect()
    {
        selected = new Maybe<GameObject>();
        onChange.Publish();
    }
}
