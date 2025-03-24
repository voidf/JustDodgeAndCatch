using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
public class DragSample : MonoBehaviour
{
    // private Vector2 position;
    LinkedList<Vector2> ll_mousepos = new();
    // private void OnMouseDrag()
    // {
    //     Debug.Log("Drag");
    //     position = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
    //     position = new Vector3(position.x, position.y, transform.position.z);
    // }

    bool m_mousedown = false;
    [SerializeField]
    int CST_mouse_pos_count = 4;
    [SerializeField]
    float CST_rotate_speed_decr = 0.05f;
    float m_acc_rad_v = 0f;
    float tf_rangle = 0f;

    Vector2[] c_pivots_offset = {
        new Vector2(0.5f,0.5f),
        new Vector2(-0.5f,0.5f),
        new Vector2(-0.5f,-0.5f),
        new Vector2(0.5f,-0.5f),
    };

    Vector2[] cf_pivots;
    Vector2 cf_mpos;
    void Awake()
    {
        cf_pivots = new Vector2[c_pivots_offset.Length];
        m_mousedown = false;
        tf_rangle = 0f;
        // m_tf_initpos = transform.position;
    }
    void CalcPivots()
    {
        for (int i = 0; i < c_pivots_offset.Length; ++i)
        {
            Vector2 x = c_pivots_offset[i];
            var p = Geometry.ToPolarCoordRadian(x);
            p.y += tf_rangle * Mathf.PI / 180f;
            cf_pivots[i] = Geometry.ToCartesianCoordRadian(p) + (Vector2)transform.position;
        }
        for (int i = 0; i < c_pivots_offset.Length; ++i)
            Debug.DrawLine(cf_pivots[i], cf_pivots[(i + 1) % cf_pivots.Length], Color.cyan, 0.02f);
    }
    void OnMouseRelease()
    {
        m_mousedown = false;
        m_acc_rad_v = 0;
        if (ll_mousepos.Count < 2) { }
        else
        {

            int ctr = 0;
            Vector2 prvpos = Vector2.zero;
            float prvang;

            foreach (var x in ll_mousepos)
            {
                Vector2 ipos = x == (Vector2)transform.position ? x + new Vector2(1e-5f, 0) : x;
                if (++ctr > 1)
                {
                    // x - prvpos
                    prvang = Vector2.SignedAngle(prvpos - (Vector2)transform.position, ipos - (Vector2)transform.position);
                    m_acc_rad_v += prvang / (ll_mousepos.Count - 1);
                }
                prvpos = ipos;
            }
        }
        ll_mousepos.Clear();
    }

    void DragRotation()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            m_mousedown = true;
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            OnMouseRelease();
            return;
        }
        else
        {
            tf_rangle += m_acc_rad_v;
            transform.rotation = Quaternion.Euler(0, 0, tf_rangle);
            // transform.Rotate(new Vector3(0, 0, m_z_rot));
            // m_acc_rad_v *= CST_rotate_speed_decr; // 指数衰减
            if (Mathf.Abs(m_acc_rad_v) <= CST_rotate_speed_decr) m_acc_rad_v = 0;
            else
                m_acc_rad_v += -Mathf.Sign(m_acc_rad_v) * CST_rotate_speed_decr;
        }
        if (m_mousedown)
        {
            m_acc_rad_v = 0;
            if (ll_mousepos.Count > 0)
            {
                var ang = Vector2.SignedAngle(ll_mousepos.Last.Value - (Vector2)transform.position, cf_mpos - (Vector2)transform.position);
                tf_rangle += ang;
                // transform.Rotate(new Vector3(0, 0, ang));
            }
            ll_mousepos.AddLast(cf_mpos);
            while (ll_mousepos.Count > CST_mouse_pos_count)
                ll_mousepos.RemoveFirst();
        }
    }

    // void ApplyTransform()
    // {
    //     Vector2 o = transform.position;
    //     Quaternion r = Quaternion.Euler(0, 0, m_tf_z_rot);

    // }

    bool m_mousedown4move = false;
    Vector2 prv_mpos = new Vector2(float.NaN, float.NaN);
    void DragMove()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            m_mousedown4move = true;
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            m_mousedown4move = false;
            prv_mpos = new Vector2(float.NaN, float.NaN);
            return;
        }
        if (m_mousedown4move)
        {
            if (!float.IsNaN(prv_mpos.x))
                transform.position += (Vector3)(cf_mpos - prv_mpos);
            prv_mpos = cf_mpos;
        }
    }

    void Update()
    {
        CalcPivots();
        cf_mpos = (Vector2)Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        if (Geometry.IsInPolygon(cf_pivots, cf_mpos)) DragMove();
        else
            DragRotation();

        // if()
        // Debug.DrawLine(c_pivots_offset[0], c_pivots_offset[1], Color.red, 0.1f);
        // Debug.DrawLine(c_pivots_offset[1], c_pivots_offset[2], Color.yellow, 0.1f);
        // Debug.DrawLine(c_pivots_offset[0], c_pivots_offset[2], Color.blue, 0.1f);
    }


}
