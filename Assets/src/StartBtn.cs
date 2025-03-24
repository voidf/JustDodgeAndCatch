using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class StartBtn : SceneMono<StartBtn>
{

    [SerializeField]
    Color idleColor = Color.white;
    [SerializeField]
    Color hoverColor = Color.gray;
    [SerializeField]
    Color clickColor = Color.cyan;
    int holding = 0;

    SpriteRenderer sr;


    Vector2[] cf_pivots; // 本帧开始的Update里计算pivots

    new void Awake()
    {
        base.Awake();
        sr = GetComponent<SpriteRenderer>();
        cf_pivots = new Vector2[Geometry.triangle_offset.Length];
    }

    void OnEnable()
    {
        sr.color = idleColor;
    }

    void Update()
    {
        var cf_mpos = (Vector2)Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        Geometry.CalcPivots(ref cf_pivots, Geometry.triangle_offset, transform);
        if (Mouse.current.leftButton.wasPressedThisFrame)
            holding = 1;
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            holding = 0;
            if (Geometry.IsInPolygon(cf_pivots, cf_mpos))
            {
                gameObject.SetActive(false);
                LevelLoader.Instance.PrepareGameStart();
                return;
            }
        }
        if (Geometry.IsInPolygon(cf_pivots, cf_mpos))
            sr.color = holding == 1 ? clickColor : hoverColor;
        else
            sr.color = idleColor;
    }
}
