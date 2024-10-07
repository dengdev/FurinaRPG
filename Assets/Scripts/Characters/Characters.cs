using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Characters
{
    public string characterName;
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefence;


    public Characters(int maxHealth, int currentHealth, int baseDefence, int currentDefence) {
        this.maxHealth = maxHealth;
        this.currentHealth = currentHealth;
        this.baseDefence = baseDefence;
        this.currentDefence = currentDefence;
    }
}
