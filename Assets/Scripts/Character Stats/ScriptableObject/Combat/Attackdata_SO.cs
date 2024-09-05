using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CreateAssetMenu �������ڴ����Զ���Ĳ˵������ Unity �Ĳ˵��п��ٴ������ ScriptableObject ʵ����
// fileName ָ��Ĭ�ϵ��ļ�����menuName ���� Unity �����˵�����ʾ��·����
[CreateAssetMenu(fileName = "New Attack", menuName = "Character Stats/Attack")]

// Attackdata_SO ��̳��� ScriptableObject��ScriptableObject �� Unity ���ڱ������ݵ��࣬
// ��ͨ�����ڶ�����ڶ������乲�����Ϸ���ݣ����繥������ֵ�����ԡ�
public class Attackdata_SO : ScriptableObject
{
    // ������Χ��ָ��һ�����ܹ�ִ�й�������Ч���롣
    public float attackRange;

    // ���ܷ�Χ��ָ���ܵ����÷�Χ������ͨ������Χ��ͬ��
    public float skillRange;

    // ������ȴʱ�䣬ָ����ʹ�ú���Ҫ�ȴ���ʱ�䣬��λ���롣
    public float coolDown;

    // ��������С�˺�ֵ��ָ��������ʱ����ɵ�����˺���
    public int minDamage;

    // ����������˺�ֵ��ָ��������ʱ����ɵ�����˺���
    public int maxDamage;

    // �������ʣ�ָ����ʱ�˺������ı��������� 2.0 ��ʾ����ʱ��������˺���
    public float criticalMultiplier;

    // �������ʣ�ָÿ�ι����ж���ʲ�����������ֵͨ���� 0 �� 1 ֮�䣬0.25 ��ʾ 25% �����ʡ�
    public float criticalChance;
}
