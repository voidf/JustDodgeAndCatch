using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 因为懒得打AB包了，这个单例放所有需要游戏运行时动态加载的prefab，音频，图片之类的资源
public class ResBank : SceneMono<ResBank>
{
    // public SongConf[] songs;
    public HitObjectConf hoconf;

    public GameObject PF_CatchGO;
    public GameObject PF_AvoidGO;
    public GameObject PF_AvoidLinear;
    public GameObject PF_FXGreenCircle;

    public Sprite[] SP_carry;
    static ResBank _me;
    static GameObject _mygo;
    public static ResBank FindMe()
    {
        if (_me == null) // 听说Find性能很差，所以这里缓存一下
            _me = FindMyGO().GetComponent<ResBank>();
        return _me;
    }
    public static GameObject FindMyGO() // 还在GO还在GO
    {
        if (_mygo == null) // 听说Find性能很差，所以这里缓存一下
            _mygo = GameObject.Find("ResBank");
        return _mygo;
    }
    public static (int, int) GetResolution()
    {
#if UNITY_EDITOR
        int w = 1920;
        int h = 1080;
#else
        int w = Screen.width; // 这两个在编辑时是Inspector的尺寸
        int h = Screen.height;
#endif
        return (w, h);
    }
}
