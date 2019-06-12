using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovementController : MonoBehaviour
{
    public AudioClip stunSound;
    public float speed = 1f;
    public float stunDuration = 0.4f;

    private AudioSource audioSource;
    private Rigidbody2D rb;
    private float dragControl = 0;
    public bool CanMove { get; private set; }
    private float inputSmoother = 0.1f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        CanMove = true;
    }

    public void Move(float xAxisInput, float yAxisInput) {
        if (CanMove) {
            inputSmoother += 0.1f;
            xAxisInput *= inputSmoother;
            yAxisInput *= inputSmoother;
            Vector2 newVelocity = Vector2.zero;
            if (xAxisInput != 0 || yAxisInput != 0) {
                newVelocity = Vector2.ClampMagnitude(new Vector2(xAxisInput, yAxisInput) * speed, speed);
            } else {
                inputSmoother = 0;
            }
            if (newVelocity.Equals(rb.velocity)) {
                dragControl = 0;
            } else {
                dragControl += 0.1f;
            }
            rb.velocity = Vector2.Lerp(rb.velocity, newVelocity, dragControl);
            if (rb.velocity != Vector2.zero) {
                float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
                Quaternion rot = Quaternion.AngleAxis(angle - 90, Vector3.forward);
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, 0.4f);
            }
        }
    }

    public void DisableMovement() {
        CanMove = false;
        rb.velocity = Vector2.zero;
    }

    public void EnableMovement() {
        CanMove = true;
    }

    public void Stun()
    {
        StartCoroutine(StunRoutine());
    }

    IEnumerator StunRoutine()
    {
        transform.DOShakePosition(stunDuration, new Vector3(1f, 1f, 0f)).SetDelay(Time.fixedDeltaTime).Play();
        audioSource.PlayOneShot(stunSound);
        float t = 0f;
        while (t < stunDuration) {
            DisableMovement();
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        EnableMovement();
    }
}
