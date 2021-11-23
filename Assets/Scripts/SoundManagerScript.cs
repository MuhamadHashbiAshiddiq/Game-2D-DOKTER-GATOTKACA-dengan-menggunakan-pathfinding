using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public static AudioClip attackSound, throwSound, jumpSound, walkSound;
    static AudioSource audioSrc;
    void Start()
    {
        attackSound = Resources.Load<AudioClip> ("attackSound");
        jumpSound = Resources.Load<AudioClip> ("jumpSound");

        audioSrc = GetComponent<AudioSource> ();
    }

    void Update()
    {

    }

    public static void PlaySound (string clip)
    {
        switch (clip) {
        case "attackSound":
            audioSrc.PlayOneShot (attackSound);
            break;
        case "jumpSound":
            audioSrc.PlayOneShot (jumpSound);
            break;
        }
    }
}
