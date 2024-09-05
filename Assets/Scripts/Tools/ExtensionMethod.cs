using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 扩展方法类，用于对 Transform 进行各种扩展操作
/// </summary>
public static class ExtensionMethod 
{
    private const float dotThreshold = 0.5f; // 夹角的阈值，夹角在60度以内即正前方120度内可以攻击到

    /// <summary>
    /// 判断角色是否正面朝向目标
    /// </summary>
    public static bool IsFacingTarget(this Transform transform, Transform target)
    {
        // 计算从当前 Transform 到目标 Transform 的向量
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize(); 

        // 计算当前 Transform 的前向向量与目标向量之间的点积
        float dot = Vector3.Dot(transform.forward, vectorToTarget);

        // 点积的值在夹角小于等于60度时会大于等于 dotThreshold
        return dot >= dotThreshold;
    }
}
