using System.Collections;
using UnityEngine;

public class PlayLevelCompleteVictorySound : OnMessage<LevelCompleted>
{
    [SerializeField] private UiSfxPlayer player;
    [SerializeField] private AudioClip clip;
    [SerializeField] private FloatReference delay = new FloatReference(0);

    protected override void Execute(LevelCompleted msg) => StartCoroutine(Play());

    private IEnumerator Play()
    {
        yield return new WaitForSeconds(delay);
        player.Play(clip);
    }
}
