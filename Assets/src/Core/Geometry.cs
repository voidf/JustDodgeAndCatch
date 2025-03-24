using UnityEngine;
using System.Runtime.CompilerServices;
using System.Linq;
// 计算几何规定使用标准极坐标系定义：x轴正方向为0°，逆时针为正
// 所有的多边形几何顶点应该以x轴逆时针旋转角度最小，按照逆时针旋转角度从小到大排序
public static class Geometry
{
    // 要求逆时针序
    public static readonly Vector2[] square_offset = {
        new Vector2(0.5f,0.5f),
        new Vector2(-0.5f,0.5f),
        new Vector2(-0.5f,-0.5f),
        new Vector2(0.5f,-0.5f),
    };

    public static readonly Vector2[] triangle_offset = {
        new Vector2(0,0.5773502691896257f), // sqrt(3)/3
        new Vector2(-0.5f,-0.28867513459481287f), // -sqrt(3)/6
        new Vector2(0.5f,-0.28867513459481287f),
    };


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 ToPolarCoordRadian(in Vector2 cartesianCoord)
    {
        var d = Mathf.Atan2(cartesianCoord.y, cartesianCoord.x);
        // if (d < 0) d += Mathf.PI * 2;
        return new Vector2(cartesianCoord.magnitude, d);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 ToPolarCoordAngle(in Vector2 cartesianCoord)
    {
        var d = Mathf.Atan2(cartesianCoord.y, cartesianCoord.x);
        // if (d < 0) d += Mathf.PI * 2;
        return new Vector2(cartesianCoord.magnitude, d * 180f / Mathf.PI);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 ToCartesianCoordRadian(in Vector2 polarCoord) => new(
        polarCoord.x * Mathf.Cos(polarCoord.y),
        polarCoord.x * Mathf.Sin(polarCoord.y)
    );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 ToCartesianCoordAngle(in Vector2 polarCoord) => new(
        polarCoord.x * Mathf.Cos(polarCoord.y * Mathf.PI / 180f),
        polarCoord.x * Mathf.Sin(polarCoord.y * Mathf.PI / 180f)
    );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cross2D(in Vector2 l, in Vector2 r) => l.x * r.y - l.y * r.x;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInPolygon(in Vector2[] pivots, in Vector2 p)
    {
        bool res = false;
        Vector2 j = pivots.Last();
        foreach (Vector2 i in pivots)
        {
            if ((i.y < p.y && j.y >= p.y || j.y < p.y && i.y >= p.y) && (i.x <= p.x || j.x <= p.x))
                res ^= i.x + (p.y - i.y) / (j.y - i.y) * (j.x - i.x) < p.x;
            j = i;
        }
        return res;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CalcPivots(ref Vector2[] cf_pivots, in Vector2[] c_pivots_offset, Transform tf)
    {
        for (int i = 0; i < c_pivots_offset.Length; ++i)
        {
            Vector2 x = c_pivots_offset[i] * (Vector2)tf.localScale;
            var p = Geometry.ToPolarCoordRadian(x);
            p.y += tf.rotation.eulerAngles.z * Mathf.PI / 180f;
            cf_pivots[i] = Geometry.ToCartesianCoordRadian(p) + (Vector2)tf.position;
        }
        // for (int i = 0; i < c_pivots_offset.Length; ++i)
        // Debug.DrawLine(cf_pivots[i], cf_pivots[(i + 1) % cf_pivots.Length], Color.cyan, 0.02f);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]

    public static float EaseOutBounce(float x)
    {
        const float n1 = 7.5625f;
        const float d1 = 2.75f;
        if (x < 1 / d1)
        {
            return n1 * x * x;
        }
        else if (x < 2 / d1)
        {
            return n1 * (x -= 1.5f / d1) * x + 0.75f;
        }
        else if (x < 2.5 / d1)
        {
            return n1 * (x -= 2.25f / d1) * x + 0.9375f;
        }
        else
        {
            return n1 * (x -= 2.625f / d1) * x + 0.984375f;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float EaseInCubic(float x) => x * x * x;

}