using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������Ҫ�����������Ҫ�̳иýӿ�
/// </summary>
public interface ISaveable {
    // �̳иýӿں��Զ�ע�ᵽSaveManager������Managerͳһ���б������
    void RegisteSaveable() {
        SaveManager.Instance.RegisterSaveable(this);
    }

    GameSaveData GenerateSaveData();

    void RestoreGameData(GameSaveData gameSaveData);
}
