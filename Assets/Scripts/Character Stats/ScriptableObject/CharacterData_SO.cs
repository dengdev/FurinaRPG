using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Data",menuName ="Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]
    public int maxHealth; // ��ɫ���������ֵ
    public int currentHealth; // ��ɫ�ĵ�ǰ����ֵ
    public int baseDefence; // ��ɫ�Ļ�������ֵ
    public int currentDefence; // ��ɫ�ĵ�ǰ����ֵ

    [Header("Kill Point")]
    public int killPoint; // ��ɫ�Ļ�ɱ���������ڼ��㾭���������Ŀ��

    [Header("Level Up")] // ��ɫ������ص����ԣ�������ҽ�ɫ��Ч
    public int currentLevel; // ��ɫ��ǰ�ȼ�
    public int maxLevel; // ��ɫ����ߵȼ�
    public int baseExp, currentExp; // ��ɫ��������Ļ�������ֵ�͵�ǰ����ֵ
    public float levelBuff; // ÿ������ʱ�����Եİٷֱ���������

    /// <summary>
    /// ���½�ɫ�ľ���ֵ��������Ƿ�ﵽ��������
    /// </summary>
    public void UpdateExp(int point)
    {
        currentExp += point; // ���Ӿ���ֵ
        if (currentExp >= baseExp) // �����ǰ����ֵ�ﵽ�򳬹����辭��ֵ
            LevelUp(); // ������������
    }

    /// <summary>
    /// ��ȡ��ǰ�ȼ���������������
    /// </summary>
    float LevelMultiplier
    {
        get { return 1 + (currentLevel - 1) * levelBuff; } // ���㵱ǰ�ȼ���������������
    }

    /// <summary>
    /// ��ҵȼ��������������������߼�
    /// </summary>
    private void LevelUp()
    {
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel); // �ȼ�����1�������Ҳ��������ȼ�
        baseExp += (int)(baseExp * LevelMultiplier); // ���ݵȼ����������������辭��ֵ

        maxHealth = (int)(maxHealth * LevelMultiplier); // ���ݵȼ����������������ֵ
        baseDefence += 1; // ÿ��������������ֵ����1��

        currentHealth = maxHealth; // �����󽫵�ǰ����ֵ�ָ����������ֵ
        currentDefence = baseDefence; // �����󽫵�ǰ����ֵ�ָ�����������ֵ
    }
}
