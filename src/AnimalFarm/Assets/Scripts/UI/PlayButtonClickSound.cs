using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class PlayButtonClickSound : MonoBehaviour
{
    [SerializeField] private AudioClip sound;
    [SerializeField] private UiSfxPlayer player;
    
    private void Awake() => GetComponent<Button>().onClick.AddListener(() => player.Play(sound));
}
