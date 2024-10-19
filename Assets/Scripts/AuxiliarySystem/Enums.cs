public enum EnemyState {
    GUARD, PATROL, CHASE, DEAD, ALERT
}

public enum SceneTransitionType { SameScene, CrossScene }
public enum TransferAnchorTag { Enter, A, B, C, D, E, F }


public enum ItemName {
    None, Coin
}

public enum GameState {
    Pause, GamePlay
}

public enum ItemQuality {
    OneStar,
    TwoStar,
    ThreeStar,
    FourStar,
    FiveStar
}

public enum ItemType {
    Weapon, // 武器
    Relic, // 遗器
    CultivationMaterial, // 培养素材
    Cuisine, // 料理
    Material, // 材料
    QuestItem // 任务道具
}