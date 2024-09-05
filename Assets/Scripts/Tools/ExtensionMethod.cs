using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��չ�����࣬���ڶ� Transform ���и�����չ����
/// </summary>
public static class ExtensionMethod 
{
    private const float dotThreshold = 0.5f; // �нǵ���ֵ���н���60�����ڼ���ǰ��120���ڿ��Թ�����

    /// <summary>
    /// �жϽ�ɫ�Ƿ����泯��Ŀ��
    /// </summary>
    public static bool IsFacingTarget(this Transform transform, Transform target)
    {
        // ����ӵ�ǰ Transform ��Ŀ�� Transform ������
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize(); 

        // ���㵱ǰ Transform ��ǰ��������Ŀ������֮��ĵ��
        float dot = Vector3.Dot(transform.forward, vectorToTarget);

        // �����ֵ�ڼн�С�ڵ���60��ʱ����ڵ��� dotThreshold
        return dot >= dotThreshold;
    }
}
