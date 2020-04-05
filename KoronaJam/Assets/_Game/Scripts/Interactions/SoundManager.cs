using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    static SoundManager _instance;
    [SerializeField] private AudioSource source;

    private void Awake()
    {
        _instance = FindObjectOfType<SoundManager>();
    }

    public static void PlaySound(AudioClip clip)
    {
        _instance.source.clip = clip;
        _instance.source.Play();
    }
}
