/// <summary>
/// ������Ҫ�����������Ҫ�̳иýӿ�
/// </summary>
public interface ISaveable {
    // �Զ�ע�ᵽ SaveManager �Խ���ͳһ�������
    public void AutoRegisteSaveable() {
        SaveManager.Instance.RegisterSaveable(this);
    }

    public GameSaveData GenerateSaveData();

    public void RestoreGameData(GameSaveData gameSaveData);
}
