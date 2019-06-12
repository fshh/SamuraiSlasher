using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillCollider : MonoBehaviour
{
    private DuelManager duelManager;
    private DashAttack dashScript;

    // Start is called before the first frame update
    void Start()
    {
        duelManager = FindObjectOfType<DuelManager>();
        dashScript = transform.root.gameObject.GetComponent<DashAttack>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // do nothing if this player is not attacking
        if (!dashScript.IsAttacking()) {
            return;
        }

        GameObject enemy = collision.gameObject.transform.root.gameObject;
        if (enemy.CompareTag("Player") && enemy != this.transform.root.gameObject) {
            DashAttack enemyDash = enemy.GetComponent<DashAttack>();
            BlockAttack enemyBlock = enemy.GetComponent<BlockAttack>();
            PlayerDeath enemyDeath = enemy.GetComponent<PlayerDeath>();

            // if players attack each other or the enemy blocks this player's attack, enter a duel
            if (enemyDash.IsAttacking() || enemyBlock.IsBlocking()) {
                // cancel attacks/blocks
                dashScript.CancelAttack();
                enemyDash.CancelAttack();
                enemyBlock.CancelBlock();

                // enter duel with enemy
                duelManager.CreateDuel(transform.root.gameObject, enemy);
            } 
            // otherwise, kill the enemy if they're not already dying
            else if (!enemyDeath.IsDying()) {
                enemyDeath.Die();
                dashScript.KillEnemy();
            }
        }
    }
}
