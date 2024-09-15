using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class, attached to audio sources used by the sound manager, send an event when the audio source have finished to reproduce sound
/// </summary>
public class MyAudioSource : MonoBehaviour
{
    public static event Action<AudioSource> AudioClipEnded;
    private AudioSource m_AudioSource;

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!m_AudioSource.isPlaying)
            AudioClipEnded?.Invoke(m_AudioSource);
    }

}
