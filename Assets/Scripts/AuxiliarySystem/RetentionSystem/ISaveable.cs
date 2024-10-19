public interface ISaveable {
    // 自动注册到 SaveManager 以进行统一保存操作
    public void AutoRegisteSaveable() {
        SaveManager.Instance.RegisterSaveable(this);
    }

    public GameSaveData GenerateSaveData();

    public void RestoreGameData(GameSaveData gameSaveData);
}
