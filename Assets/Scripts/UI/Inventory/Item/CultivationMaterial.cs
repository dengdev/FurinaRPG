using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色培养素材，比如角色经验书，武器强化石，遗器经验
/// </summary>
public class CultivationMaterial : Item {
    public int experience;
    public override void Use(int quality) {
        base.Use(quality);

        Debug.Log("使用了经验道具" + experience);
        int addExperience = experience * quality;
        GameManager.Instance.playerData.AddExp(addExperience);
    }

}
