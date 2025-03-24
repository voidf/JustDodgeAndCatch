using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIProgress : MonoBehaviour
{
    [SerializeField]
    GameObject begin, pivot, end;
    void Start()
    {
        pivot.transform.position = begin.transform.position;
    }
    void FixedUpdate()
    {
        var plr = AudioManager.Instance.bgmPlayer;
        if (plr.clip == null || plr.clip.length == 0) return;
        var progress = plr.time;
        var len = plr.clip.length;
        pivot.transform.position = Vector3.Lerp(begin.transform.position, end.transform.position, progress / len);
    }
}
