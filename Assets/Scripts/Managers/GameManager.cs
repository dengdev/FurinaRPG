using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;

    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();

    /// <summary>
    /// 注册玩家
    /// </summary>
    /// <param name="player"></param>
    public void RegisterPlayer(CharacterStats player)
    {
        playerStats = player;
    }

    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }

    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    /// <summary>
    /// 观察者广播
    /// </summary>
    public void NotifyObservers()
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }




}
