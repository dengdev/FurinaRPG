using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState : IPlayerState {
    private PlayerController player;
    public void Enter(PlayerController player) {
        this.player = player;
        if(player.playerIsDizzy == true) {
            player.GetComponent<Animator>().SetTrigger("Dizzy");
            Debug.Log("玩家被击晕了！");

        } else {
            player.playerIsHIt = true;

            player.GetComponent<Animator>().SetTrigger("Hit");
            Debug.Log("玩家受到普通攻击");
        }
       
    }

    public void Exit() {
        player.playerIsDizzy = false;
        player.playerIsHIt = false;
    }

    public void Update() {
    }
}
