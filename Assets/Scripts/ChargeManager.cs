using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeManager : MonoBehaviour
{
    public float currentCharge = 0;
    public float rechargeTime;

    private MovementController move;
    private InputManager input;
    private DashAttack dash;
    private BlockAttack block;
    private void Start() {
        move = GetComponent<MovementController>();
        input = GetComponent<InputManager>();
        dash = GetComponent<DashAttack>();
        block = GetComponent<BlockAttack>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!dash.IsAttacking() && !block.IsBlocking() && move.CanMove && input.canReceiveInput)
        {
           addCharge(Time.deltaTime * (100 / rechargeTime));
        }
        int count = 0;
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Player"))
        {
            count++;
        }
        if(count == 2 && rechargeTime > .5f)
        {
            rechargeTime -= (Time.deltaTime * (.1f));
        }
    }
    public void addCharge(float value)
    {
        currentCharge = Mathf.Clamp(currentCharge + value, 0, 100);
    }
    public bool removeCharge(float value)
    {
        if(currentCharge - value < 0)
        {
            return false;
        }
        else
        {
            currentCharge -= value;
            return true;
        }
    }
}
