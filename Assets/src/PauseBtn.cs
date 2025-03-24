using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
public class PauseBtn : SceneMono<PauseBtn>
{

    [SerializeField]
    Color idleColor = Color.white;
    [SerializeField]
    Color hoverColor = Color.gray;
    [SerializeField]
    Color clickColor = Color.cyan;

    [SerializeField]
    SpriteRenderer sq1, sq2, sqret, resumesr;
    // [SerializeField]
    // Animation resumeani;
    // Animator resumeanim;
    Vector2[] sq1_pivots = new Vector2[Geometry.square_offset.Length],
                sq2_pivots = new Vector2[Geometry.square_offset.Length],
                sqret_pivots = new Vector2[Geometry.square_offset.Length],
                resume_pivots = new Vector2[Geometry.triangle_offset.Length];

    [SerializeField]
    Vector3 T_enterPos, T_exitPos, T_enterScale, T_exitScale;
    [SerializeField]
    float T_duration;

    new void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        sq2.color = sq1.color = idleColor;
        resumesr.gameObject.SetActive(false);
        sqret.gameObject.SetActive(false);
    }
    bool isPause = false;
    void OnClickPause()
    {
        sq1.gameObject.SetActive(false);
        sq2.gameObject.SetActive(false);
        Time.timeScale = 0f;
        LevelLoader.gameFreeze = 1;
        AudioManager.Instance.PauseBGM();
        sqret.gameObject.SetActive(true);
        resumesr.gameObject.SetActive(true);
        resumesr.transform.localPosition = T_enterPos;
        resumesr.transform.localScale = T_enterScale;
        resumesr.transform.DOLocalMove(T_exitPos, T_duration).SetUpdate(true).SetEase(Ease.OutSine);
        resumesr.transform.DOScale(T_exitScale, T_duration).SetUpdate(true).SetEase(Ease.OutSine);
        resumesr.transform.rotation = Quaternion.Euler(0, 0, -90);
        rot_done_time = Time.unscaledTime + T_duration;
        C_resume = idleColor;
        isReverse = false;
        isRotating = false;
        isPause = true;
    }
    void OnClickReturn()
    {
        // OnClickResume();
        LevelLoader.gameFreeze = 0;
        AudioManager aud = AudioManager.Instance;
        aud.bgmPlayer.pitch = 1;
        aud.bgmPlayer.pitch = 1;
        Time.timeScale = 1;
        isPause = false;

        sq1.gameObject.SetActive(true);
        sq2.gameObject.SetActive(true);
        sqret.gameObject.SetActive(false);
        resumesr.gameObject.SetActive(false);
        gameObject.SetActive(false);
        // sq1.gameObject.SetActive(true);
        // sq2.gameObject.SetActive(true);
        // resumesr.gameObject.SetActive(false);
        LevelLoader.Instance.ReturnToTitle();
    }

    void OnClickResume()
    {
        OnEnable();
        isPause = false;
        AudioManager aud = AudioManager.Instance;

        LevelLoader.gameFreeze = 0;
        LevelLoader ll = LevelLoader.Instance;
        if (isReverse)
        {
            aud.bgmPlayer.pitch = -1;
            ll.timeScaler = -1;
        }
        else
        {
            aud.bgmPlayer.pitch = 1;
            ll.timeScaler = 1;
        }

        ll.gameStartTimeOffset = Time.time;
        ll.runningTimeOffset = aud.bgmPlayer.time;
        Time.timeScale = 1;
        aud.ResumeBGM();
        isPause = false;
        sq1.gameObject.SetActive(true);
        sq2.gameObject.SetActive(true);
        resumesr.gameObject.SetActive(false);
        sqret.gameObject.SetActive(false);
        if (aud.bgmPlayer.time == 0) // 重开
        {
            aud.bgmPlayer.Play();
            aud.bgmPlayer.Pause();
            LevelLoader.Instance.PrepareGameStart();
        }
        // gameObject.SetActive(true);
    }

    Vector2 cf_mpos;
    int holding = 0;

    void ToPauseHandler()
    {
        Geometry.CalcPivots(ref sq1_pivots, Geometry.square_offset, sq1.transform);
        Geometry.CalcPivots(ref sq2_pivots, Geometry.square_offset, sq2.transform);
        if (Mouse.current.leftButton.wasReleasedThisFrame)
            if (Geometry.IsInPolygon(sq1_pivots, cf_mpos) || Geometry.IsInPolygon(sq2_pivots, cf_mpos))
            {
                OnClickPause();
                return;
            }
        if (Geometry.IsInPolygon(sq1_pivots, cf_mpos) || Geometry.IsInPolygon(sq2_pivots, cf_mpos))
            sq2.color = sq1.color = holding == 1 ? clickColor : hoverColor;
        else
            sq2.color = sq1.color = idleColor;
    }
    bool isReverse = false;
    bool isRotating = false;
    Vector2 mousePosBeforeRotate;
    float eulerBeforeRotate;
    float rot_done_time = 0;
    Color C_resume;

    [SerializeField]
    float rot_duration = 0.44f;
    [SerializeField]
    Color reverseModeColor = Color.red;
    void ToResumeHandler()
    {
        Geometry.CalcPivots(ref resume_pivots, Geometry.triangle_offset, resumesr.transform);
        Geometry.CalcPivots(ref sqret_pivots, Geometry.square_offset, sqret.transform);

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (Geometry.IsInPolygon(sqret_pivots, cf_mpos))
            {
                OnClickReturn();
                return;
            }
            if (Geometry.IsInPolygon(resume_pivots, cf_mpos))
            {
                OnClickResume();
                return;
            }
            if (isRotating)
            {
                isRotating = false;
                rot_done_time = Time.unscaledTime + rot_duration;
                float curz = resumesr.transform.rotation.eulerAngles.z % 360;
                if (135 > curz && curz > 45)
                {
                    resumesr.transform.DORotate(new Vector3(0, 0, 90), rot_duration)
                    .SetUpdate(true)
                    .SetEase(Ease.OutSine)
                    .OnComplete(() =>
                    {
                        resumesr.color = C_resume = reverseModeColor;
                        isReverse = true;
                    });
                }
                else
                {
                    resumesr.transform.DORotate(new Vector3(0, 0, -90), rot_duration)
                    .SetUpdate(true)
                    .SetEase(Ease.OutSine)
                    .OnComplete(() =>
                    {
                        resumesr.color = C_resume = idleColor;
                        isReverse = false;
                    });
                }
            }
        }

        if (Geometry.IsInPolygon(sqret_pivots, cf_mpos))
        {
            sqret.color = V_ALinear.jsabPink * (holding == 0 ? 1f : 0.6f);
        }
        else sqret.color = Color.white;

        if (Geometry.IsInPolygon(resume_pivots, cf_mpos))
        {
            resumesr.color = C_resume * 0.8f;
        }
        else
        {
            if (Time.unscaledTime - rot_done_time > 0)
            {
                if (isRotating)
                {
                    float ang = Vector2.SignedAngle(
                        mousePosBeforeRotate - (Vector2)resumesr.transform.position,
                        cf_mpos - (Vector2)resumesr.transform.position
                        );
                    resumesr.transform.rotation = Quaternion.Euler(0, 0, eulerBeforeRotate + ang);

                }
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    isRotating = true;
                    mousePosBeforeRotate = cf_mpos;
                    eulerBeforeRotate = resumesr.transform.rotation.eulerAngles.z;
                }
            }
            resumesr.color = C_resume;
        }
    }
    void Update()
    {
        cf_mpos = (Vector2)Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!isPause) OnClickPause();
            else OnClickResume();
            return;
        }
        // if (Keyboard.current.spaceKey.wasPressedThisFrame)
        // {
        //     OnClickResume();
        //     Debug.Log($"SPACE DOWN RESUME");
        //     return;
        // }

        if (Mouse.current.leftButton.wasPressedThisFrame)
            holding = 1;
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            holding = 0;
        }
        if (isPause) ToResumeHandler();
        else ToPauseHandler();
    }
}
