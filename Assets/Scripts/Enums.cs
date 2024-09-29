public enum EnemyState {
    GUARD, PATROL, CHASE, DEAD, ALERT
}

public enum ItemName {
    None, Coin
}

public enum GameState {
    Pause, GamePlay
}

public enum ItemQuality {
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public enum ItemType {
    Weapon, // 武器
    Relic, // 遗器
    CultivationMaterial, // 培养素材
    Cuisine, // 料理
    Material, // 材料
    QuestItem // 任务道具
}