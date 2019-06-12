using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class IntroText : MonoBehaviour
{
    public float textScaleDuration;
    public Vector3 maxScale;
    public string[] texts;
    public AudioClip[] sounds;

    private InputManager[] inputs;
    private Text text;
    private AudioSource audioSource;
    private int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();

        ResetScale();

        Sequence introSequence = DOTween.Sequence()
                            .OnStart(IntroStart)
                            .OnKill(IntroEnd);

        Tweener scaleText = transform.DOScale(maxScale, textScaleDuration)
                            .SetLoops(texts.Length)
                            .OnStart(ChangeText)
                            .OnStepComplete(ChangeText);

        introSequence.Append(scaleText);
        introSequence.Play();
    }

    void ResetScale()
    {
        transform.localScale = new Vector3(0.01f, 0.01f, 1f);
    }

    void IntroStart()
    {
        inputs = FindObjectsOfType<InputManager>();
        foreach (InputManager i in inputs)
        {
            i.canReceiveInput = false;
        }
    }

    void IntroEnd()
    {
        text.enabled = false;
        foreach (InputManager i in inputs)
        {
            i.canReceiveInput = true;
        }
    }

    void ChangeText()
    {
        if (count >= texts.Length)
        {
            return;
        }
        text.text = texts[count];
        audioSource.PlayOneShot(sounds[count]);
        count++;
    }
}
