using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DashAttack : MonoBehaviour
{
    // state tracker
    private bool isAttacking = false;

    // audio data
    public AudioClip dashSound, slashSound, killSound;
    private AudioSource audioSource;

    // katana sprites/objects
    public SpriteRenderer katanaSide;
    public GameObject katanaSlash;

    // attack settings/parameters
    public float dashHitWidth = 3f;
    public float dashDistance = 30f;
    public float dashDuration = 0.25f;
    public float slashAngle = 60f;
    public float slashDuration = 0.2f;
    public float slashVulnerabilityDuration = 0.1f;
    public float hitStunTimeScale = 0.01f;
    public float hitStunDuration = 0.3f;

    // component references
    private Rigidbody2D rb;
    private MovementController movement;
    private ChargeManager charge;
    private BoxCollider2D dashCollider;
    private CircleCollider2D bodyCollider;
    private TrailRenderer trail;

    // tweening objects
    private Sequence attackSequence;
    private Tweener dash;
    private Tweener slash;

    // dash update data
    private Vector2 destPos;
    private Vector2 lastPos;
    private Vector2 stopPos = new Vector2(Mathf.NegativeInfinity, Mathf.NegativeInfinity);

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<MovementController>();
        charge = GetComponent<ChargeManager>();
        dashCollider = GetComponent<BoxCollider2D>();
        bodyCollider = GetComponent<CircleCollider2D>();
        trail = GetComponent<TrailRenderer>();
        trail.enabled = false;

        katanaSlash.transform.localRotation = Quaternion.AngleAxis(slashAngle / 2, transform.forward);
    }
    
    // At the end of each frame while the player is attacking, check to see if the player has collided with a wall
    private void FixedUpdate() 
    {
        if (isAttacking) {
            DashUpdate();
        }
    }

    // Causes the player to perform their dash attack over a specified distance
    public void Attack(float distance) {
        attackSequence = DOTween.Sequence()
                .OnStart(AttackStart)
                .OnKill(AttackEnd);

        destPos = rb.position + (Vector2)(transform.up.normalized * distance);

        dash = rb.DOMove(destPos, dashDuration, false)
                .OnStart(DashStart)
                .OnKill(DashEnd)
                .SetEase(Ease.OutExpo)
                .SetUpdate(UpdateType.Fixed);

        slash = katanaSlash.transform.DOLocalRotate(new Vector3(0f, 0f, -slashAngle / 2), slashDuration)
                .OnStart(SlashStart)
                .OnKill(SlashEnd)
                .SetEase(Ease.OutQuart)
                .SetUpdate(UpdateType.Fixed);

        attackSequence.Append(dash).Append(slash);
        attackSequence.Play();
    }

    // Causes the player to perform their dash attack
    public void Attack() {
        Attack(dashDistance);
    }

    // Causes the player to perform only the dash component of the attack, over a specified distance
    public void Dash(float distance) {
        attackSequence = DOTween.Sequence()
                .OnStart(AttackStart)
                .OnKill(AttackEnd);

        destPos = rb.position + (Vector2)(transform.up.normalized * distance);

        dash = rb.DOMove(destPos, dashDuration, false)
                .OnStart(DashStart)
                .OnKill(DashEnd)
                .SetEase(Ease.OutExpo)
                .SetUpdate(UpdateType.Fixed);

        attackSequence.Append(dash);
        attackSequence.Play();
    }

    // Causes the player to perform only the slash component of the attack
    public void Slash() {
        attackSequence = DOTween.Sequence()
                .OnStart(AttackStart)
                .OnKill(AttackEnd);

        slash = katanaSlash.transform.DOLocalRotate(new Vector3(0f, 0f, -slashAngle / 2), slashDuration)
                .OnStart(SlashStart)
                .OnKill(SlashEnd)
                .SetEase(Ease.OutQuart)
                .SetUpdate(UpdateType.Fixed);

        attackSequence.Append(slash);
        attackSequence.Play();
    }

    // causes this player's attack to be canceled early
    public void CancelAttack() {
        if (isAttacking) {
            attackSequence.Kill();
        }
    }
    
    // Called when the entire attack (dash + slash) begins
    // Prevents the player from moving, sets isAttacking to true, and enables trail
    void AttackStart() 
    {
        movement.DisableMovement();
        isAttacking = true;
        trail.enabled = true;
    }
    
    // Called when the entire attack (dash + slash) ends
    // Allows the player to move, sets isAttacking to false, and disables trail
    void AttackEnd()
    {
        movement.EnableMovement();
        isAttacking = false;
        trail.enabled = false;
    }

    // Called when the dash part of the attack begins
    // Disables the circle collider defining the player's body so that they may pass through other players during the dash
    // Initializes lastPos to be the player's current position, for the purposes of DashUpdate()
    // Plays a dashing sound
    // Calculates if the player will hit a wall during their dash and sets stopPos to be the centroid of that collision, also for the purposes of DashUpdate()
    void DashStart()
    {
        bodyCollider.enabled = false;

        lastPos = rb.position;

        audioSource.PlayOneShot(dashSound);

        RaycastHit2D hit;
        hit = Physics2D.CircleCast(rb.position, bodyCollider.radius, transform.up,
              Vector2.Distance(rb.position, destPos), 1 << LayerMask.NameToLayer("Wall"));

        if (hit) {
            stopPos = hit.centroid;
        } else {
            stopPos = new Vector2(Mathf.NegativeInfinity, Mathf.NegativeInfinity);
        }
    }

    // Called when the dash part of the attack ends
    // Resets the dash hit collider
    // Re-enables the player's body collider so they collide with walls and players as normal
    void DashEnd()
    {
        ResetDashCollider();
        bodyCollider.enabled = true;
    }

    // Called every frame while the player is attacking
    // Checks to see if the player has collided with a wall during their dash
    //   If the player has hit a wall, end the attack early and stun the player
    //   Else, update the dash hit collider to occupy the space between the player's last position and current position
    void DashUpdate()
    {
        if (stopPos.magnitude != Mathf.Infinity && Vector2.Distance(stopPos, destPos) > Vector2.Distance(rb.position, destPos))
        {
            attackSequence.Kill();
            rb.position = stopPos;
            movement.Stun();
            return;
        }

        float diff = Vector2.Distance(rb.position, lastPos);

        dashCollider.size = new Vector2(dashHitWidth, diff);
        dashCollider.offset = new Vector2(0, 0.5f * -diff);

        lastPos = rb.position;
    }

    // Resets the collider used for hit detection during the dash to be very small and centered on the player
    void ResetDashCollider()
    {
        dashCollider.size = new Vector2(0.1f, 0.1f);
        dashCollider.offset = Vector2.zero;
    }

    // Called when the slash part of the attack begins
    // Enables the slashing katana object so it is visible and ready to interact with enemies
    // Makes the side katana invisible
    // Plays a slashing sound
    void SlashStart()
    {
        katanaSlash.SetActive(true);
        katanaSide.enabled = false;
        audioSource.PlayOneShot(slashSound);
    }

    // Called when the slash part of the attack ends
    // Disables the slashing katana object and resets its rotation
    // Makes the side katana visible
    void SlashEnd()
    {
        katanaSlash.SetActive(false);
        katanaSlash.transform.localRotation = Quaternion.AngleAxis(slashAngle / 2, transform.forward);

        katanaSide.enabled = true;

        //yield return new WaitForSecondsRealtime(slashVulnerabilityDuration);
    }

    // Returns true if the player is currently using their attack, false otherwise
    public bool IsAttacking()
    {
        return isAttacking;
    }

    // Called when this player kills an opponent
    // Activates the hitstun effect on this player, which slows down time for them temporarily
    // Also activates a screenshake effect on the camera
    public void KillEnemy() {
        audioSource.PlayOneShot(killSound);
        GetComponent<ChargeManager>().addCharge(100);
        Camera.main.transform.DOShakePosition(hitStunDuration, new Vector3(3f, 3f, 0f), vibrato:15);
        StartCoroutine(HitStunRoutine());
    }

    // Slows down time by a specified amount for a specified amount of time
    IEnumerator HitStunRoutine() {
        attackSequence.timeScale = hitStunTimeScale;
        yield return new WaitForSecondsRealtime(hitStunDuration);
        attackSequence.timeScale = 1f;
    }
}
