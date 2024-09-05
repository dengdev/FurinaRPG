using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CreateAssetMenu 属性用于创建自定义的菜单项，能在 Unity 的菜单中快速创建这个 ScriptableObject 实例。
// fileName 指定默认的文件名，menuName 是在 Unity 创建菜单中显示的路径。
[CreateAssetMenu(fileName = "New Attack", menuName = "Character Stats/Attack")]

// Attackdata_SO 类继承自 ScriptableObject，ScriptableObject 是 Unity 用于保存数据的类，
// 它通常用于定义可在多个对象间共享的游戏数据，例如攻击的数值和特性。
public class Attackdata_SO : ScriptableObject
{
    // 攻击范围，指玩家或敌人能够执行攻击的有效距离。
    public float attackRange;

    // 技能范围，指技能的作用范围，和普通攻击范围不同。
    public float skillRange;

    // 技能冷却时间，指技能使用后需要等待的时间，单位是秒。
    public float coolDown;

    // 攻击的最小伤害值，指攻击命中时能造成的最低伤害。
    public int minDamage;

    // 攻击的最大伤害值，指攻击命中时能造成的最大伤害。
    public int maxDamage;

    // 暴击倍率，指暴击时伤害提升的倍数，例如 2.0 表示暴击时造成两倍伤害。
    public float criticalMultiplier;

    // 暴击几率，指每次攻击有多大几率产生暴击，数值通常在 0 到 1 之间，0.25 表示 25% 暴击率。
    public float criticalChance;
}
