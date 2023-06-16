using UnityEngine;

[CreateAssetMenu]
public sealed class GameMusicPlayer : ScriptableObject
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private string lastSongName;
    
    public void Init(AudioSource source)
    {
        StopMusicIfPlaying();
        musicSource = source;
    }

    public void InitIfNeeded(AudioSource source)
    {
        if (musicSource == null)
            Init(source);
    }

    public string LastSongName => lastSongName;

    public void PlaySelectedMusicLooping(AudioClip clipToPlay)
    {
        if (musicSource == null)
        {
            Debug.LogError($"nameof(musicSource) has not been initialized");
            return;
        }
        
        if (musicSource.isPlaying && musicSource.clip != null && musicSource.clip.name.Equals(clipToPlay.name))
            return;
        
        StopMusicIfPlaying();
        lastSongName = clipToPlay.name;
        musicSource.clip = clipToPlay;
        musicSource.loop = true;
        musicSource.Play();
    }

    private void StopMusicIfPlaying()
    {
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();
    }
}
