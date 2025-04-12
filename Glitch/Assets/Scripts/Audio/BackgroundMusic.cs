using System;
using System.Collections;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic Instance;

    public string[] musicTrackNames;
    public string bossMusic;
    private int currentTrackIndex = 0;
    private AudioSource currentSource;
    public bool isBossFight = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (musicTrackNames.Length == 0)
        {
            Debug.LogWarning("No music tracks assigned!");
            return;
        }

        PlayNextTrack();
    }

    void Update()
    {
        if (currentSource != null && !currentSource.isPlaying && !Settings.IsOpen)
        {
            if (!isBossFight)
                PlayNextTrack();
            else
            {
                PlayBossTrack();
            }
        }
    }

    public void StopBackgroundMusic()
    {
        currentSource.Stop();
    }

    public void PlayBossTrack()
    {
        isBossFight = true;
        StopBackgroundMusic();
        currentSource = AudioManager.GetSound(bossMusic);

        if (currentSource == null)
        {
            return;
        }
        currentSource.Play();
    }

    private void PlayNextTrack()
    {
        string nextTrackName = musicTrackNames[currentTrackIndex];
        currentSource = AudioManager.GetSound(nextTrackName);

        if (currentSource == null && !Settings.IsOpen)
        {
            Debug.LogWarning("nu exista: " + nextTrackName);
            return;
        }

        currentSource.Play();
        currentTrackIndex = (currentTrackIndex + 1) % musicTrackNames.Length;
    }
}
