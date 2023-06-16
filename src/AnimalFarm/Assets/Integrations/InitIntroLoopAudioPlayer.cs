using UnityEngine;

namespace Integrations
{
    public class InitIntroLoopAudioPlayer : CrossSceneSingleInstance
    {
        [SerializeField] private IntroLoopAudioPlayer player;
        
        protected override string UniqueTag => "Music";
        
        protected override void OnAwake()
        {
            player.Init();
        }
    }
}