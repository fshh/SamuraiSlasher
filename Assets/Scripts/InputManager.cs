using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [System.Serializable]
    public enum PlayerNumber { ONE = 1, TWO = 2, THREE = 3, FOUR = 4 }
    public PlayerNumber playerNumber;
    public Text playerNumberText;
    public bool canReceiveInput = true;

    private Player player;

    private MovementController movement;
    private DashAttack dashAttack;
    private BlockAttack blockAttack;
    private ChargeManager charge;
    private EmoteManager emotes;

    private static string horizontalInput = "Move Horizontal";
    private static string verticalInput = "Move Vertical";
    private static string dashInput = "Attack";
    private static string blockInput = "Block";
    private static string emote1 = "Emote1";
    private static string emote2 = "Emote2";
    private static string emote3 = "Emote3";
    private static string emote4 = "Emote4";

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<MovementController>();
        dashAttack = GetComponent<DashAttack>();
        blockAttack = GetComponent<BlockAttack>();
        charge = GetComponent<ChargeManager>();
        emotes = GetComponent<EmoteManager>();

        int pnum = (int)playerNumber;
        player = ReInput.players.GetPlayer(pnum - 1);
        playerNumberText.text = "P " + pnum;
    }

    // Update is called once per frame
    void Update()
    {
        if (canReceiveInput)
        {
            if ((player.GetButtonDown(dashInput) || player.GetAxisRaw(dashInput) > 0f) && !blockAttack.IsBlocking() && !dashAttack.IsAttacking() && charge.removeCharge(100f))
            {
                dashAttack.Attack();
            }
            else if ((player.GetButtonDown(blockInput) || player.GetAxisRaw(blockInput) > 0f) && !blockAttack.IsBlocking() && !dashAttack.IsAttacking() && charge.removeCharge(50f))
            {
                blockAttack.Block();
            }

            if (player.GetButtonDown(emote1))
            {
                emotes.DisplayEmote(0);
            }
            else if (player.GetButtonDown(emote2))
            {
                emotes.DisplayEmote(1);
            }
            else if (player.GetButtonDown(emote3))
            {
                emotes.DisplayEmote(2);
            }
            else if (player.GetButtonDown(emote4))
            {
                emotes.DisplayEmote(3);
            }
        }
    }

    private void FixedUpdate()
    {
        if (canReceiveInput)
        {
            movement.Move(player.GetAxisRaw(horizontalInput), player.GetAxisRaw(verticalInput));
        }
    }
}
