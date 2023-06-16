using UnityEngine;

public class PlayUiSoundOnDisable : MonoBehaviour
{
    [SerializeField] private UiSfxPlayer player;
    [SerializeField] private AudioClip clip;

    private void OnDisable() => player.Play(clip);
}
