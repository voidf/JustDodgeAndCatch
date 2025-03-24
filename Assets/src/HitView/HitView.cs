using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using OsuParsers.Beatmaps.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

using Newtonsoft.Json; // 推荐使用Newtonsoft.Json for Unity

public abstract class HitViewBase : MonoBehaviour
{
    public int botype;
    public static readonly Type[] botypelut = {
        // typeof(HitObjectView<>).MakeGenericType(typeof(BeatEntity)),  // 0
        typeof(BeatEntity),  // 0
        typeof(E_Catch),        // 1
        typeof(E_ALinear),    // 2
        };
    public abstract void ToGameTime(int gameTimeMS);
    public abstract void BindHitObject(HitObject ho);
    [NonSerialized] public bool judged = false;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 OsuPosition2WorldPosition(float x, float y)
    {
        var (w, h) = ResBank.GetResolution();
        // Debug.Log($"H:{h} W:{w}");
        float ws = (float)w / 640f; // 512 * 384 4:3
        float hs = (float)h / 480f; // 640 * 480
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(
            new Vector3((x + 64) * ws, (y + 56) * hs, 0) // https://osu.ppy.sh/wiki/en/Client/Playfield
        );
        worldPos.z = 0;
        return worldPos;
    }
    public abstract BeatEntity GetBO();
    public abstract void SetBO(BeatEntity _bo);
    public abstract void ApplyCachedBO();
}
public class HitView<T> : HitViewBase where T : BeatEntity, new()
{
    public T bo;
    public override void SetBO(BeatEntity _bo) { bo = _bo as T; bo.type = botype; }
    public override BeatEntity GetBO() { bo.type = botype; return bo; }
    public HitView()
    {
        botype = 0;
    }

    public override void BindHitObject(HitObject ho)
    {
        // entity = ho;
        bo.x = (int)ho.Position.X;
        bo.y = (int)ho.Position.Y;
        bo.start_time = ho.StartTime;
        ApplyCachedBO();     // bo -> go
    }

    void OnValidate()
    {
        ApplyCachedBO();
        var ll = LevelLoader.FindMe();
        ll.debugGameTimeMS = bo.start_time + 50;
        ll.ApplyToAllHOV(bo.start_time);
    }

    public override void ApplyCachedBO()
    {
        transform.position = OsuPosition2WorldPosition(bo.x, bo.y);
        judged = false;
        // Debug.Log($"APPLY {transform.position} {OsuPosition2WorldPosition(bo.x, bo.y)}");
    }

    public override void ToGameTime(int gameTimeMS) { }

}




