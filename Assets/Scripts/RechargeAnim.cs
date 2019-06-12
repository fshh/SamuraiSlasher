using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RechargeAnim : MonoBehaviour
{
    public ChargeManager charge;
    private Image circleImage;
    // Start is called before the first frame update
    void Start()
    {
        circleImage = this.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        circleImage.fillAmount = charge.currentCharge / 100;
    }
}
