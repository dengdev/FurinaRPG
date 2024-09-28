using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState : IPlayerState {
    private PlayerController player;
    public void Enter(PlayerController player) {
        this.player = player;
        if(player.playerIsDizzy == true) {
            player.GetComponent<Animator>().SetTrigger("Dizzy");
            Debug.Log("��ұ������ˣ�");

        } else {
            player.playerIsHIt = true;

            player.GetComponent<Animator>().SetTrigger("Hit");
            Debug.Log("����ܵ���ͨ����");
        }
       
    }

    public void Exit() {
        player.playerIsDizzy = false;
        player.playerIsHIt = false;
    }

    public void Update() {
    }
}
