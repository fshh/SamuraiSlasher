using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmoteManager : MonoBehaviour
{
    public Image emoteImage;
    public Sprite[] emotes = new Sprite[4];
    public float displayTime = 2f;

    private bool displaying = false;

    public void DisplayEmote(int index)
    {
        if (!displaying)
        {
            Display(emotes[index]);
            Invoke("StopDisplaying", displayTime);
        }
    }

    void Display(Sprite sprite)
    {
        displaying = true;
        emoteImage.sprite = sprite;
        emoteImage.enabled = true;
    }

    void StopDisplaying()
    {
        displaying = false;
        emoteImage.enabled = false;
    }
}
