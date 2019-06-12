using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillCounter : MonoBehaviour
{
    private Text counter;

    private void Start()
    {
        counter = GetComponent<Text>();
    }

    public void UpdateKills(int kills)
    {
        counter.text = kills.ToString();
    }
}
