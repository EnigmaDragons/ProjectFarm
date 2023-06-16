using UnityEngine;

public class PlayLevelResetSoundAfterInit : OnMessage<LevelReset>
{
    [SerializeField] private UiSfxPlayer player;
    [SerializeField] private AudioClip clip;

    private bool _isSetup;

    protected override void Execute(LevelReset msg)
    {
        if (_isSetup)
            player.Play(clip);
        _isSetup = true;
    }
}