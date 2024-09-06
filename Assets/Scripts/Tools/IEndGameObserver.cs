
/// <summary>
/// 广播玩家死亡跟游戏结束
/// </summary>
public interface IEndGameObserver
{
    /// <summary>
    /// 玩家死亡的逻辑
    /// </summary>
    void EndNotify();
    
}
