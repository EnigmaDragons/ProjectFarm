using System;
using UnityEngine;

[Serializable]
public sealed class AudioClipWithVolume
{
    public AudioClip clip;
    public FloatReference volume;
}
