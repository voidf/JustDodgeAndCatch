using DG.Tweening;
using Newtonsoft.Json;
using OsuParsers.Beatmaps.Objects;
using UnityEngine;

public class V_Catch : HitView<E_Catch>
{
    public CircleCollider2D c2d;
    public V_Catch() { botype = 1; }

    static Vector3 originalScale = new Vector3(0.18f, 0.18f, 0.18f);
    // void Awake()
    // {
    //     originalScale = transform.localScale;
    // }
    public override void ApplyCachedBO()
    {
        base.ApplyCachedBO();
        // var _sr = GetComponent<SpriteRenderer>();
        // _sr.color = bo.color;
    }

    public override void BindHitObject(HitObject ho)
    {
        base.BindHitObject(ho);
    }
    public override void ToGameTime(int gameTimeMS)
    {
        if (judged) return;
        ResBank r = ResBank.FindMe();
        var _sr = GetComponent<SpriteRenderer>();
        int fadeInPivot = bo.start_time - r.hoconf.CatchGOFadeIn;
        int lifeEndTime = bo.start_time + r.hoconf.CatchGOLifeTime;
        if (gameTimeMS < fadeInPivot)
        {
            gameObject.SetActive(false);
            c2d.enabled = false;
            return;
        }
        else if (gameTimeMS < bo.start_time)
        {
            c2d.enabled = true;
            gameObject.SetActive(true);
            var x = Mathf.InverseLerp(fadeInPivot, bo.start_time, gameTimeMS);
            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, x);
            return;
        }
        else if (gameTimeMS < lifeEndTime)
        {
            c2d.enabled = true;
            Color c = _sr.color;
            c.a = Mathf.InverseLerp(lifeEndTime, bo.start_time, gameTimeMS);
            _sr.color = c;
            return;
        }
        else if (lifeEndTime <= gameTimeMS)
        {
            gameObject.SetActive(false);
            c2d.enabled = false;
            return;
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.TryGetComponent<FollowMouse>(out FollowMouse fm))
        {
            judged = true;
            var rb = ResBank.FindMe();
            var fxgo = Instantiate(rb.PF_FXGreenCircle, transform.position, transform.rotation);
            fxgo.transform.localScale = transform.lossyScale;
            fxgo.transform.DOMove(new Vector3(-9.19f, -5.21f, 0), 0.2f).SetUpdate(true).SetEase(Ease.OutSine).OnComplete(() =>
            {
                Destroy(fxgo);
            });
            UIScore.Instance.DeltaScore(1);
            gameObject.SetActive(false);
        }
    }
}