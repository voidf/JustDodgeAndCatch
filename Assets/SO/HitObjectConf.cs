using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HitObjectConf", menuName = "ScriptableObjects/HitObjectConf", order = 10)]
public class HitObjectConf : ScriptableObject
{
    public int CatchGOFadeIn = 600;
    public int CatchGOLifeTime = 700;
    public int AvoidGOFadeIn = 600;
    public int AvoidLinearWarning = 1000;
}
