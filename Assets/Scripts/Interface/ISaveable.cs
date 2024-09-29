using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有需要保存的内容需要继承该接口
/// </summary>
public interface ISaveable {
    // 继承该接口后自动注册到SaveManager，由其Manager统一进行保存操作
    void RegisteSaveable() {
        SaveManager.Instance.RegisterSaveable(this);
    }

    GameSaveData GenerateSaveData();

    void RestoreGameData(GameSaveData gameSaveData);
}
