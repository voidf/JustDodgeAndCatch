using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    这玩意会在对象池用Get方法时添加到被管理的GO上
*/
public class GameObjectPoolInfo : MonoBehaviour
{
    /// <summary>
    /// 对象显示的持续时间，若=0，则不隐藏
    /// </summary>
    [HideInInspector] public float lifetime = 0;
    /// <summary>
    /// 所属对象池的唯一id
    /// </summary>
    [HideInInspector] public string poolName;

    WaitForSeconds m_waitTime;

    void Awake()
    {
        if (lifetime > 0)
        {
            m_waitTime = new WaitForSeconds(lifetime);
        }
    }

    void OnEnable()
    {
        if (lifetime > 0)
        {
            StartCoroutine(CountDown(lifetime)); // 用协程去做倒计时这件事
        }
    }

    IEnumerator CountDown(float lifetime)
    {
        yield return m_waitTime;
        //将对象加入对象池
        GameObjectPoolManager.Instance.RemoveGameObject(poolName, gameObject);
    }
}
