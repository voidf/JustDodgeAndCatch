using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMono<T> : MonoBehaviour where T : SceneMono<T>
{
    public static T Instance { get; protected set; }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else
            Debug.LogError($"单例模式类{typeof(T).Name}脚本被挂载了多次，挂载对象名：{name}");
    }
}
