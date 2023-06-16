using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public sealed class SceneBackgroundMusic : MonoBehaviour
{
    [SerializeField] private AudioClip music;
    [SerializeField] private AudioClip[] altMusic = new AudioClip[0]; 
    [SerializeField] private GameMusicPlayer musicPlayer;
    [SerializeField] private FloatReference delayDuration = new FloatReference(0);

    private void Start()
    {
        StartCoroutine(ExecuteAfterDelay(delayDuration, 
            () => musicPlayer.PlaySelectedMusicLooping(
                altMusic.Length > 0 
                    ? altMusic.Concat(music)
                        .Where(x => !x.name.Equals(musicPlayer.LastSongName))
                        .Random()
                    : music)));
    }
    
    private IEnumerator ExecuteAfterDelay(float duration, Action action)
    {
        yield return new WaitForSeconds(duration);
        action();
    }
}
