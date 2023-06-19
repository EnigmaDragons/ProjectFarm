using UnityEngine;

[CreateAssetMenu]
public class Character : ScriptableObject
{
    [SerializeField] private string name;
    [SerializeField] private Sprite bust;
    [SerializeField] private Sprite alternateArtBust;
    [SerializeField] private BoolReference useAlternateArt = new BoolReference(false);
    [SerializeField] private bool useStaticVisualEffect = false;

    public string Name => name;
    public Sprite Bust => alternateArtBust != null && useAlternateArt ? alternateArtBust : bust;
    public bool UseStatic => useStaticVisualEffect;
}
