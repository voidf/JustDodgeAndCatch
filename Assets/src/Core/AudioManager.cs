using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SceneMono<AudioManager>
{
    float seVolume = 1f;
    float bgmVolume = 1f;

    public void ReloadConfig()
    {
        var config = JsonConfigManager.LoadConfig();
        seVolume = config.effectVolume;
        bgmVolume = config.musicVolume;
        bgmPlayer.volume = bgmVolume;
        ChangeAllSource(seVolume);
    }

    public AudioSource bgmPlayer;

    public Stack<AudioSource> UnusedAS = new Stack<AudioSource>();
    public List<AudioSource> ActiveAS = new List<AudioSource>();

    public new void Awake()
    {
        base.Awake();
        if (bgmPlayer == null)
        {
            if (this.gameObject.TryGetComponent<AudioSource>(out bgmPlayer))
                bgmPlayer = GetComponent<AudioSource>();
            else
                bgmPlayer = this.gameObject.AddComponent<AudioSource>();
        }
        ReloadConfig();
    }

    public void FixedUpdate()
    {
        for (int i = 0; i < ActiveAS.Count; i++)
        {
            var audio = ActiveAS[i];
            if (audio == null)
            {
                ActiveAS[i] = ActiveAS[^1];
                ActiveAS.RemoveAt(ActiveAS.Count - 1);
                // return;
            }
            if (!audio.isPlaying)
            {
                UnusedAS.Push(audio);
                ActiveAS[i] = ActiveAS[^1];
                ActiveAS.RemoveAt(ActiveAS.Count - 1);
            }
        }
    }
    public void ChangeAllSource(float volume)
    {
        for (int i = 0; i < ActiveAS.Count; i++)
        {
            ActiveAS[i].volume = volume;
        }
    }

    public void PlaySE(AudioClip targetClip)
    {
        AudioSource temp;
        bool ok = UnusedAS.TryPeek(out temp);

        if (ok && temp != null)
        {
            temp = UnusedAS.Pop();
            temp.clip = targetClip;
            ActiveAS.Add(temp);
            temp.volume = seVolume;
            temp.loop = false;
            temp.Play();
        }
        else
        {
            GameObject temps = new GameObject();
            AudioSource audioTemp = temps.AddComponent<AudioSource>();
            audioTemp.clip = targetClip;
            audioTemp.volume = seVolume;
            audioTemp.loop = false;
            audioTemp.Play();
            ActiveAS.Add(audioTemp);
        }
    }

    public void PlayBGM(AudioClip clip) // 调试暂用，之后看有没有必要用
    {
        bgmPlayer.clip = clip;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.Play();
        //bgmPlayer.loop = true;
    }
    public void StopBGM() // 暂时做一个停止音乐的函数
    {
        bgmPlayer.Stop();
    }
    public void PauseBGM()
    {
        bgmPlayer.Pause();
    }

    public void ResumeBGM()
    {
        bgmPlayer.UnPause();
    }

    public void SetBGMPitch(float pitch)
    {
        bgmPlayer.pitch = pitch;
    }

    // SongConf currentSongConf;
    // void ChangeLoopAfterBGMEnd()
    // {
    //     bgmPlayer.clip = currentSongConf.loopClip;
    //     bgmPlayer.time = currentSongConf.loopOffset;
    //     bgmPlayer.Play();
    //     Invoke(nameof(ChangeLoopAfterBGMEnd), bgmPlayer.clip.length - bgmPlayer.time);
    // }
    // public void LoadLoopConfig(SongConf cfg)
    // {
    //     if(cfg.loopClip == null)
    //     {
    //         Debug.LogError("No loop clip found in SongConf: " + cfg.name);
    //         return;
    //     }
    //     currentSongConf = cfg;
    //     CancelInvoke(nameof(ChangeLoopAfterBGMEnd));
    //     float reverseTime = bgmPlayer.clip.length - bgmPlayer.time;
    //     Invoke(nameof(ChangeLoopAfterBGMEnd), reverseTime);
    // }
}
