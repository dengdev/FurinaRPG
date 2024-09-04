using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 判定敌人攻击角度
/// </summary>
public static class ExtensionMethod 
{
    private const float dotThreshold = 0.5f; // 夹角在60度以内，即敌人正前方120度内可以攻击到
    public static bool IsFacingTarget(this Transform transform, Transform target)
    {
        var vectorToTarget=target.position-transform.position;
        vectorToTarget.Normalize();
        float dot=Vector3.Dot( transform.forward,vectorToTarget);
        return dot >= dotThreshold;
    }
}
