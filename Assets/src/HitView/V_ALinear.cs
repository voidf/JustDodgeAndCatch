using System;
using Newtonsoft.Json;
using OsuParsers.Beatmaps.Objects;
using UnityEngine;

public class V_ALinear : HitView<E_ALinear>
{
    public override void BindHitObject(HitObject ho)
    {
        base.BindHitObject(ho);
        Slider sl = ho as Slider;
        bo.end_x = (int)sl.SliderPoints[^1].X;
        bo.end_y = (int)sl.SliderPoints[^1].Y;
        bo.end_time = sl.EndTime;
    }

    public V_ALinear() { botype = 2; }
    Transform eff, warn, old;

    public static readonly Color jsabPink = new Color(252f / 255f, 31 / 255f, 111 / 255f, 1f);
    public override void ApplyCachedBO()
    {
        base.ApplyCachedBO();
        eff = transform.Find("Eff");
        warn = transform.Find("Warn");
        // old = transform.Find("old");
        Vector2 s = OsuPosition2WorldPosition(bo.x, bo.y);
        Vector2 t = OsuPosition2WorldPosition(bo.end_x, bo.end_y);
        var d = t - s;
        warn.position = eff.position = Vector2.LerpUnclamped(s, t, 0.5f);
        warn.localScale = eff.localScale = new Vector3(1f, d.magnitude, 1f);
        warn.rotation = eff.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, d));
    }

    public override void ToGameTime(int gameTimeMS)
    {
        var _sr = GetComponent<SpriteRenderer>();
        var r = ResBank.FindMe();
        int fadeInPivot = bo.start_time - r.hoconf.AvoidLinearWarning;
        int enterFXStart =
            // Math.Min(
            Math.Min(bo.end_time, bo.start_time + 200)
            // ,(bo.end_time - bo.start_time) / 10 + bo.start_time)
            ;
        int lifeEndTime = bo.end_time;
        int exitFXEnd = lifeEndTime + 100;

        if (gameTimeMS < fadeInPivot)
        {
            gameObject.SetActive(false);
            warn.gameObject.SetActive(false);
            eff.gameObject.SetActive(false);
            return;
        }
        else if (gameTimeMS < bo.start_time)
        {
            gameObject.SetActive(true);
            warn.gameObject.SetActive(true);
            var wsr = warn.gameObject.GetComponent<SpriteRenderer>();
            eff.gameObject.SetActive(false);
            // Color c = _sr.color;
            float l = Mathf.InverseLerp(fadeInPivot, bo.start_time, gameTimeMS);
            wsr.color = new Color(wsr.color.r, wsr.color.g, wsr.color.b, Mathf.LerpUnclamped(0, 0.1f, l));
            // old.position = Vector3.Lerp(
            //     OsuPosition2WorldPosition(bo.x, bo.y),
            //     OsuPosition2WorldPosition(bo.end_x, bo.end_y),
            //     l
            // );
            // _sr.color = c;
            return;
        }
        else if (gameTimeMS < enterFXStart)
        {
            gameObject.SetActive(true);
            eff.gameObject.SetActive(true);
            warn.gameObject.SetActive(true);
            float t = Geometry.EaseOutBounce(Mathf.InverseLerp(bo.start_time, enterFXStart, gameTimeMS));
            eff.GetComponent<SpriteRenderer>().color = Color.LerpUnclamped(Color.white, jsabPink, t);
            eff.localScale = new Vector3(t, eff.localScale.y, eff.localScale.z);
        }
        else if (gameTimeMS < lifeEndTime)
        {
            eff.GetComponent<SpriteRenderer>().color = jsabPink;
            eff.localScale = new Vector3(1f, eff.localScale.y, eff.localScale.z);
            gameObject.SetActive(true);
            eff.gameObject.SetActive(true);
            warn.gameObject.SetActive(true);
            Color c = _sr.color;
            c.a = Mathf.InverseLerp(lifeEndTime, bo.start_time, gameTimeMS);
            _sr.color = c;
            return;
        }
        else if (gameTimeMS < exitFXEnd)
        {
            gameObject.SetActive(true);
            eff.gameObject.SetActive(true);
            warn.gameObject.SetActive(true);
            float t = 1f - Geometry.EaseInCubic(Mathf.InverseLerp(lifeEndTime, exitFXEnd, gameTimeMS));
            eff.localScale = new Vector3(t, eff.localScale.y, eff.localScale.z);
        }
        else if (lifeEndTime <= gameTimeMS)
        {
            gameObject.SetActive(false);
            return;
        }
    }
}
