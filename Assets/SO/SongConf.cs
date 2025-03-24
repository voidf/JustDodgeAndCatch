using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SongConf", menuName = "ScriptableObjects/SongConf", order = 10)]
public class SongConf : ScriptableObject
{
    public float bpm = 135f;
    public float delayPlay = 0f;
    public AudioClip bgm;
    // public List<AudioClip> judgeSE = new();
    public AudioClip[] hitSamples;
    public string bmPath;
}
