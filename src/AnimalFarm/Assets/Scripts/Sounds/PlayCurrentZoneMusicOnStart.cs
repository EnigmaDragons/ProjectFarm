using E7.Introloop;
using UnityEngine;

public class PlayCurrentZoneMusicOnStart : MonoBehaviour
{
     [SerializeField] private IntroloopAudio defaultMusic;
     [SerializeField] private CurrentZone currentZone;
     [SerializeField] private IntroLoopAudioPlayer player;

     void Start()
     {
          var music = currentZone.Zone.MusicTheme != null ? currentZone.Zone.MusicTheme : defaultMusic;
          player.PlaySelectedMusicLooping(music);
     }
}
