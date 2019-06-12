using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [System.Serializable]
    public enum PlayerNumber { ONE = 1, TWO = 2, THREE = 3, FOUR = 4 }
    public PlayerNumber playerNumber;
    public Text playerNumberText;
    public bool canReceiveInput = true;

    private MovementController movement;
    private DashAttack dashAttack;
    private BlockAttack blockAttack;
    private ChargeManager charge;
    private EmoteManager emotes;

    private string horizontalInput;
    private string verticalInput;
    private string dashInput;
    private string blockInput;
    private string dPadHorizontal;
    private string dPadVertical;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<MovementController>();
        dashAttack = GetComponent<DashAttack>();
        blockAttack = GetComponent<BlockAttack>();
        charge = GetComponent<ChargeManager>();
        emotes = GetComponent<EmoteManager>();

        int pnum = (int)playerNumber;
        horizontalInput = "Horizontal_P" + pnum;
        verticalInput = "Vertical_P" + pnum;
        dashInput = "Dash_P" + pnum;
        blockInput = "Block_P" + pnum;
        dPadHorizontal = "DPadHorizontal_P" + pnum;
        dPadVertical = "DPadVertical_P" + pnum;

        playerNumberText.text = "P " + pnum;
    }

    // Update is called once per frame
    void Update()
    {
        if (canReceiveInput)
        {
            if ((Input.GetButtonDown(dashInput) || Input.GetAxisRaw(dashInput) > 0f) && !blockAttack.IsBlocking() && !dashAttack.IsAttacking() && charge.removeCharge(100f))
            {
                dashAttack.Attack();
            }
            else if ((Input.GetButtonDown(blockInput) || Input.GetAxisRaw(blockInput) > 0f) && !blockAttack.IsBlocking() && !dashAttack.IsAttacking() && charge.removeCharge(50f))
            {
                blockAttack.Block();
            }

            if (Input.GetAxisRaw(dPadHorizontal) > 0f)
            {
                emotes.DisplayEmote(0);
            }
            else if (Input.GetAxisRaw(dPadHorizontal) < 0f)
            {
                emotes.DisplayEmote(1);
            }
            else if (Input.GetAxisRaw(dPadVertical) > 0f)
            {
                emotes.DisplayEmote(2);
            }
            else if (Input.GetAxisRaw(dPadVertical) < 0f)
            {
                emotes.DisplayEmote(3);
            }
        }
    }

    private void FixedUpdate() {
        if (canReceiveInput)
        {
            movement.Move(Input.GetAxisRaw(horizontalInput), Input.GetAxisRaw(verticalInput));
        }
    }
}
