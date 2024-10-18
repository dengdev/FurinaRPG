using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState : IPlayerState {
    private PlayerController player;
    private Queue<float> hitTimestamps;

    private float hitDuration = 1.0f;
    private  float invincibleDuration = 5.0f;
    private bool isInvincible = false;
    private float invincibleStartTime;

    private  int hitFrequency = 3;
    public void Enter(PlayerController player) {
        this.player = player;
        hitTimestamps = player.hitTimestamps;

        if (!isInvincible) {
            hitTimestamps.Enqueue(Time.time);
            if (hitTimestamps.Count > hitFrequency) {
                hitTimestamps.Dequeue();
            }
        }

        if (ShouldEnterInvincibleState()) {
            isInvincible = true;
            invincibleStartTime = Time.time;
            // TODO：可以播放无敌特效
        }


        if (!isInvincible) {
            if (player.playerIsDizzy) {
                player.GetComponent<Animator>().SetTrigger("Dizzy");
            } else {
                player.playerIsHIt = true;
                player.GetComponent<Animator>().SetTrigger("Hit");
            }
        }

        if (isInvincible && Time.time - invincibleStartTime >= invincibleDuration) {
            isInvincible = false;

            if (player.playerIsDizzy) {
                player.GetComponent<Animator>().SetTrigger("Dizzy");
            } else {
                player.playerIsHIt = true;
                player.GetComponent<Animator>().SetTrigger("Hit");
            }
        }

    }

    public void Update() {

        if (isInvincible) {
            player.ChangeState(new IdleState());

        }

        hitDuration -= Time.deltaTime;
        if (hitDuration <= 0) {
            player.ChangeState(new IdleState());
        }
    }

    public void Exit() {
        player.playerIsDizzy = false;
        player.playerIsHIt = false;
    }

    private bool ShouldEnterInvincibleState() {
        if(hitTimestamps.Count<hitFrequency) return false;
        // 检查最近三次受击的时间间隔
        return  Time.time - hitTimestamps.Peek() < 3.0f;
    }
}
