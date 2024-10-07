using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class EnemyData : Characters
{
    public int killPoint;

    public EnemyData(int maxHealth, int currentHealth, int baseDefence, int currentDefence, int killPoint) : base(maxHealth, currentHealth, baseDefence, currentDefence) {
        this.killPoint = killPoint;
    }
}
