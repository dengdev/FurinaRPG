using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// �ж����˹����Ƕ�
/// </summary>
public static class ExtensionMethod 
{
    private const float dotThreshold = 0.5f; // �н���60�����ڣ���������ǰ��120���ڿ��Թ�����
    public static bool IsFacingTarget(this Transform transform, Transform target)
    {
        var vectorToTarget=target.position-transform.position;
        vectorToTarget.Normalize();
        float dot=Vector3.Dot( transform.forward,vectorToTarget);
        return dot >= dotThreshold;
    }
}
