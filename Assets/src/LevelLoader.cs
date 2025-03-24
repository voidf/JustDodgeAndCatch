using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using OsuParsers.Beatmaps;
using OsuParsers.Beatmaps.Objects;
using OsuParsers.Decoders;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

public class LevelLoader : SceneMono<LevelLoader>
{
    // public List<HitObjectViewBase> hitObjectViews = new();
    public static int gameFreeze = 0;
    public static bool triggerGameStart = false;
    public int level;
    public int debugGameTimeMS = 0;


    public float runningTimeOffset = 0;
    public float timeScaler = 1f;
    [NonSerialized]
    public float gameStartTimeOffset = float.NaN;
    public static List<SongConf> song_conf_list = new();
    public static Dictionary<string, SongConf> name2song_conf = new();
    public static List<OsuParsers.Beatmaps.Beatmap> bms = new();
    public static List<SMap> sms = new();
    public Beatmap loadedBM;

    public SMap loadedSM;
    static LevelLoader _me;
    static GameObject _mygo;
    public static LevelLoader FindMe()
    {
        if (_me == null) // 听说Find性能很差，所以这里缓存一下
            _me = FindMyGO().GetComponent<LevelLoader>();
        return _me;
    }
    public static GameObject FindMyGO() // 还在GO还在GO
    {
        if (_mygo == null) // 听说Find性能很差，所以这里缓存一下
            _mygo = GameObject.Find("LevelLoader");
        return _mygo;
    }
    void OnValidate()
    {
        ApplyToAllHOV(debugGameTimeMS);
    }
    public static void InitSongsFromSA()
    {
        string[] directories = Directory.GetDirectories(Application.streamingAssetsPath);

        foreach (string dir in directories)
        {
            string folderName = new DirectoryInfo(dir).Name;
            string folderNameAbs = Path.Combine(Application.streamingAssetsPath, folderName);
            string[] files = Directory.GetFiles(folderNameAbs);
            foreach (string file in files)
            {
                if (file.EndsWith(".osu"))
                {
                    var p = Path.Combine(folderNameAbs, file);
                    OsuParsers.Beatmaps.Beatmap beatmap = BeatmapDecoder.Decode(p);
                    Debug.Log($"Loaded {p}, idx: {bms.Count}, cnt:{beatmap.HitObjects.Count}");
                    // name2song_conf.Add(folderName, song_conf);
                    // song_conf_list.Add(song_conf);
                    beatmap.GeneralSection.AudioFilename = Path.Combine(folderNameAbs, beatmap.GeneralSection.AudioFilename);
                    // foreach (var hit in beatmap.HitObjects)
                    // {
                    //     Debug.Log($"{hit.Position} {hit.GetType()} {hit.StartTime} {hit.EndTime}");
                    // }
                    bms.Add(beatmap);
                }
                else if (file.EndsWith(".ini"))
                {
                    var p = Path.Combine(folderNameAbs, file);
                    var sm = new SMap
                    {
                        filePath = p
                    };
                    sms.Add(sm);
                }
            }
        }
    }
    GameObject[] GetAllBeatView()
    {
        GameObject[] ret = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; ++i) ret[i] = transform.GetChild(i).gameObject;
        return ret;
    }

    public void RemoveAllBeatView()
    {
        foreach (var c in GetAllBeatView())
            DestroyImmediate(c);
    }
    public AudioClip EnsureAudioFileReady(string audioFileAbs)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(audioFileAbs, AudioType.MPEG))
        {
            www.SendWebRequest();

            while (!www.isDone)
            {
                if (www.result != UnityWebRequest.Result.Success && www.result != UnityWebRequest.Result.InProgress)
                {
                    Debug.LogError(www.error);
                    break;
                }
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip audio = DownloadHandlerAudioClip.GetContent(www);
                // Debug.Log($"AUDIO {audio == null} {wd} {bm.GeneralSection.AudioFilename}");
                // AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                // audioSource.clip = audio;
                // audioSource.Play();
                var am = AudioManager.Instance; am.PlayBGM(audio); am.PauseBGM(); am.bgmPlayer.time = 0;


            }
        }
        return null;
    }
    public void LoadBeatmap(OsuParsers.Beatmaps.Beatmap bm)
    {
        ResBank rb = ResBank.FindMe();
        loadedSM = new()
        {
            audioFile = Path.GetFileName(bm.GeneralSection.AudioFilename)
        };
        var d = Path.GetDirectoryName(bm.GeneralSection.AudioFilename);
        loadedSM.filePath = Path.Combine(d, "sm.ini");

        for (int i = 0; i < bm.HitObjects.Count; ++i)
        {
            HitObject ho = bm.HitObjects[i];
            GameObject go = null;
            if (ho is HitCircle)
            {
                go = Instantiate(rb.PF_CatchGO, transform);
            }
            else if (ho is Slider)
            {
                go = Instantiate(rb.PF_AvoidLinear, transform);
            }
            if (go != null)
            {
                var hov = go.GetComponent<HitViewBase>();
                hov.BindHitObject(ho);
                go.SetActive(false);
                // hitObjectViews.Add(hov);
            }
        }
        loadedBM = bm;
    }
    public void LoadBeatmap() => LoadBeatmap(bms[level]);
    new void Awake()
    {
        base.Awake();
        InitSongsFromSA();
    }
    void Start()
    {
        ReturnToTitle();
    }

    public void PrepareGameStart()
    {
        LevelLoader.triggerGameStart = true;
        PauseBtn.Instance.gameObject.SetActive(true);
        FollowMouse.Instance.gameObject.SetActive(true);
        FollowMouse.Instance.transform.localScale = Vector3.one;
        UIScore.Instance.gameObject.SetActive(true);
        UIScore.Instance.UpdateScore(0);
    }
    public void OnGameStart()
    {
        gameStartTimeOffset = Time.time;
        AudioManager.Instance.ResumeBGM();
        Debug.Log($"GAMESTART {gameStartTimeOffset}");
    }

    public void ReturnToTitle()
    {
        RemoveAllBeatView();
        loadedSM = new();
        loadedSM.filePath = sms[level].filePath;
        LoadShapeMap();
        // LoadBeatmap(bms[level]);
        string ad = Path.Combine(Path.GetDirectoryName(loadedSM.filePath), loadedSM.audioFile);
        EnsureAudioFileReady(ad);
        StartBtn.Instance.gameObject.SetActive(true);
        PauseBtn.Instance.gameObject.SetActive(false);
    }

    void Update()
    {
        if (triggerGameStart)
        {
            triggerGameStart = false;
            OnGameStart();
            return;
        }
        if (gameFreeze > 0) return;
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            OnGameStart();
        }
        if (!float.IsNaN(gameStartTimeOffset))
        {
            int timeMS =
            Mathf.CeilToInt(((Time.time - gameStartTimeOffset) * timeScaler + runningTimeOffset) * 1000f);
            //Mathf.CeilToInt(AudioManager.Instance.bgmPlayer.time * 1000f);
            ApplyToAllHOV(timeMS);
        }
    }
    public void ApplyToAllHOV(int timeMS)
    {
        foreach (var x in GetAllBeatView())
            x.GetComponent<HitViewBase>().ToGameTime(timeMS);
    }

    public void SyncToDisk()
    {
        loadedSM.beatObjects.Clear();
        foreach (var x in GetAllBeatView())
        {
            loadedSM.beatObjects.Add(x.GetComponent<HitViewBase>().GetBO());
        }
        loadedSM.beatObjects.Sort((x, y) => x.start_time.CompareTo(y.start_time));
        loadedSM.Dump2File();
    }
    public void LoadShapeMap()
    {
        if (loadedSM == null) loadedSM = new();
        else
        {
            RemoveAllBeatView();
            var rb = ResBank.FindMe();
            loadedSM.LoadFromFile();
            for (int i = 0; i < loadedSM.beatObjects.Count; ++i)
            {
                BeatEntity bo = loadedSM.beatObjects[i];
                GameObject go = null;
                if (bo is E_Catch)
                {
                    go = Instantiate(rb.PF_CatchGO, transform);
                }
                else if (bo is E_ALinear)
                {
                    go = Instantiate(rb.PF_AvoidLinear, transform);
                }
                if (go != null)
                {
                    var hov = go.GetComponent<HitViewBase>();
                    hov.SetBO(bo);
                    hov.ApplyCachedBO();
                    go.SetActive(false);
                    // hitObjectViews.Add(hov);
                }
            }
        }
    }
    public void ExtendSlider()
    {
        foreach (var x in GetAllBeatView())
        {
            if (x.TryGetComponent(out V_ALinear v))
            {
                var mx = (v.bo.x + v.bo.end_x) / 2;
                var my = (v.bo.y + v.bo.end_y) / 2;
                v.bo.x = mx + (v.bo.x - mx) * 100;
                v.bo.y = my + (v.bo.y - my) * 100;
                v.bo.end_x = mx + (v.bo.end_x - mx) * 100;
                v.bo.end_y = my + (v.bo.end_y - my) * 100;
                v.ApplyCachedBO();
            }
        }
    }
}

// string wd = Path.GetDirectoryName(bm.GeneralSection.AudioFilename);

// #if UNITY_EDITOR
//             // Load the ScriptableObject from the constructed path in Editor mode
//             ScriptableObject scriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(scriptableObjectPath);
// #else
//             // In non-Editor mode, we need to load assets differently (this is a placeholder and should be replaced with actual loading logic)
//             // This is because AssetDatabase methods are only available in the Editor
//             ScriptableObject scriptableObject = Resources.Load<ScriptableObject>("Scripts/" + folderName); // Example using Resources.Load
// #endif