using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
public class FollowMouse : SceneMono<FollowMouse>
{
    static Vector2 cf_mpos;
    static Color srColor;
    new void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);
        srColor = sr.color;
        hitps.Stop();
    }

    [SerializeField]
    float followMargin = 54f;
    [SerializeField]
    ParticleSystem hitps;
    [SerializeField]
    SpriteRenderer sr;
    [SerializeField]
    float blinkScaler = 10f;

    float immune = 0;
    void Update()
    {
        if (LevelLoader.gameFreeze > 0) return;
        // Debug.Log(Mouse.current.position.value);
        // 0~1920*1080
        var (w, h) = ResBank.GetResolution();
        Vector2 followPos = Mouse.current.position.ReadValue();

        // Clamp the mouse position to be within the screen boundaries minus the margin
        followPos.x = Mathf.Clamp(followPos.x, followMargin / 1920 * w, w - followMargin / 1920 * w);
        followPos.y = Mathf.Clamp(followPos.y, followMargin / 1080 * h, h - followMargin / 1080 * h);
        cf_mpos = (Vector2)Camera.main.ScreenToWorldPoint(followPos);
        transform.position = cf_mpos;
        immune -= Time.unscaledDeltaTime;
        if (immune > 0)
        {
            sr.color = new Color(srColor.r, srColor.b, srColor.b, srColor.a * (0.5f + 0.5f * Mathf.Cos(immune * blinkScaler)));
        }
        else sr.color = srColor;
    }
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("AvoidTag") && immune <= 0)
        {
            hitps.Play();
            // transform.localScale = 
            immune = 1.4f;
            transform.DOScale((transform.localScale.x - 0.25f) * Vector3.one, 1f).SetUpdate(true).SetEase(Ease.OutBounce);
        }
    }
}
