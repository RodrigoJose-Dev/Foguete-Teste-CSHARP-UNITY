using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; } 

    private AudioSource rocketFX;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        rocketFX = GetComponent<AudioSource>();
    }

    public void StartRocketAudio()
    {
        rocketFX.Play();
    }

    public void StopRocketAudio()
    {
        rocketFX.volume = 0.2f;
        
        rocketFX.Stop();
    }
}
