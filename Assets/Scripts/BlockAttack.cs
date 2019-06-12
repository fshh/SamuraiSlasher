using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlockAttack : MonoBehaviour
{
    public GameObject katanaBlock;
    public SpriteRenderer katanaSide;
    public AudioClip blockSound;
    public float blockRotationAngle;
    public float blockDuration;
    public float vulnerabilityDuration;

    private MovementController movement;
    private AudioSource audioSource;
    private Vector3 blockRotation;
    private bool isBlocking = false;

    private Sequence blockSequence;
    private Tweener block;
    private Tweener blockWait;
    private Tweener unblock;
    private Tweener unblockWait;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<MovementController>();
        audioSource = GetComponent<AudioSource>();
        blockRotation = new Vector3(0f, 0f, blockRotationAngle);
    }

    // Causes this player to block attacks for a brief amount of time
    public void Block() {
        blockSequence = DOTween.Sequence()
                .OnStart(BlockSequenceStart)
                .OnKill(BlockSequenceEnd);

        block = katanaBlock.transform.DOLocalRotate(blockRotation, blockDuration / 6f)
                .SetEase(Ease.OutExpo)
                .SetUpdate(UpdateType.Fixed);

        blockWait = katanaBlock.transform.DOLocalRotate(blockRotation, 5f * blockDuration / 6f)
                .OnKill(BlockEnd)
                .SetUpdate(UpdateType.Fixed);

        unblock = katanaBlock.transform.DOLocalRotate(Vector3.zero, vulnerabilityDuration)
                .SetEase(Ease.OutCubic)
                .SetUpdate(UpdateType.Fixed);

        blockSequence.Append(block).Append(blockWait).Append(unblock);
        blockSequence.Play();
    }

    // cancel's this player's block early
    public void CancelBlock() {
        if (isBlocking) {
            blockSequence.Kill();
        }
    }

    private void BlockSequenceStart() {
        movement.DisableMovement();
        isBlocking = true;
        katanaBlock.SetActive(true);
        katanaSide.enabled = false;
        audioSource.PlayOneShot(blockSound);
    }

    private void BlockSequenceEnd() {
        movement.EnableMovement();
        isBlocking = false;
        katanaBlock.transform.localEulerAngles = Vector3.zero;
        katanaBlock.SetActive(false);
        katanaSide.enabled = true;
    }

    private void BlockEnd() {
        isBlocking = false;
    }

    public bool IsBlocking() {
        return isBlocking;
    }
}
