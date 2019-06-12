using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerDeath : MonoBehaviour
{
    private MovementController movement;
    private DashAttack dashAttack;

    private bool isDying = false;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<MovementController>();
        dashAttack = GetComponent<DashAttack>();
    }

    // Kills this player
    public void Die() {
        isDying = true;
        movement.DisableMovement();
        GetComponent<InputManager>().canReceiveInput = false;
        StartCoroutine(DeathRoutine());
    }

    // Disables player movement, causes them to shake, disables their gameObject after a delay, and decrements the player count
    IEnumerator DeathRoutine() {
        transform.DOShakePosition(dashAttack.hitStunDuration, new Vector3(1f, 1f, 0f), vibrato: 40, fadeOut: false).SetDelay(Time.fixedDeltaTime).Play();
        yield return new WaitForSecondsRealtime(dashAttack.hitStunDuration);
        RoundManager.playersAlive -= 1;
        gameObject.SetActive(false);
    }

    public bool IsDying() {
        return isDying;
    }
}
