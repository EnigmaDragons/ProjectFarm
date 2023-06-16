using UnityEngine;

public class PlayUiSoundOnEnable : MonoBehaviour
{
    [SerializeField] private UiSfxPlayer player;
    [SerializeField] private AudioClip clip;

    private void OnEnable() => player.Play(clip);
}