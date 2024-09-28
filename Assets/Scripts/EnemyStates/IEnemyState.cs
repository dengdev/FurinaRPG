public interface IEnemyState {
    void Enter(EnemyController enemy);
    void Update();
    void Exit();
}