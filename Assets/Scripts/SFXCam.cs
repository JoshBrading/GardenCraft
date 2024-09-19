using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXCam : MonoBehaviour
{
    public AudioClip throwSFX;
    public AudioClip[] moveSFX;

    public AudioSource audioSource;

    public void OnMove()
    {
        int i = Random.Range(0, moveSFX.Length);
        audioSource.clip = moveSFX[i];
        audioSource.Play();
    }

    public void OnThrow()
    {
        audioSource.clip = throwSFX;
        audioSource.Play();
    }
}
