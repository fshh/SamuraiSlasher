using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomSoundPlayer : MonoBehaviour
{
    public List<AudioClip> sounds;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        if (sounds.Count <= 0) { return; }

        audioSource.PlayOneShot(sounds[Random.Range(0, sounds.Count)]);
    }
}
