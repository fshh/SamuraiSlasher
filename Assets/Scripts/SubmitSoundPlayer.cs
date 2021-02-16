using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SubmitSoundPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        audioSource.Play();
        Destroy(gameObject, 10f);
    }
}
