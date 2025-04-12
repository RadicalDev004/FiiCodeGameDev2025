    using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class AudioManager : MonoBehaviour
{
    private static List<string> pausedSounds = new List<string>();
    public Sound[] sounds;
    public static AudioManager Instance;
    [Range(0f, 1f)]

#if UNITY_ANDROID
    public static bool IsPc = false;
#elif UNITY_STANDALONE_WIN
    public static bool IsPc = true;
#endif

    void Awake()
    {
        if (Instance == null) Instance = this;

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.loop = s.loop;
            s.source.playOnAwake = false;
        }
    }



    void Update()
    {
       
    }

    public static void UpdateVolume()
    {
        foreach (Sound s in Instance.sounds)
        {
            if (s.RespectVolume)
            {
                s.source.volume = (float)(!s.SoundEffect ? PlayerPrefs.GetFloat("Volume") : PlayerPrefs.GetFloat("SoundEffects")) * s.VolumeMultiplier;
            }
        }
    }

    public AudioSource GetSoundByName(string name)
    {
        Sound d = Array.Find(sounds, sound => sound.name == name);

        return d.source;
    }

    public void ChangeSoundVolume(string name, float volume)
    {
        Sound d = Array.Find(sounds, sound => sound.name == name);
        if (d == null)
            return;

        d.source.volume = volume;
    }

    public void PlaySound(string name)
    {
        //Debug.Log("se incearca: " + name);

        Sound d = Array.Find(sounds, sound => sound.name == name);
        if (d == null)
            return;

        //Debug.Log("se playeaza: " + name);
        //d.source.pitch = 1f;
        d.source.Play();
    }

    public void PauseSound(string name)
    {
        Sound d = Array.Find(sounds, sound => sound.name == name);
        if (d == null)
            return;

        //d.source.pitch = 1f;
        d.source.Pause();
    }

    public void UnPauseSound(string name)
    {
        Sound d = Array.Find(sounds, sound => sound.name == name);
        if (d == null)
            return;

        //d.source.pitch = 1f;
        d.source.UnPause();
    }
    public void PlayReversedSound(string name)
    {
        Sound d = Array.Find(sounds, sound => sound.name == name);
        if (d == null)
            return;

        d.source.pitch = -1;
        d.source.Play();
    }
    public void StopSound(string name)
    {
        Sound d = Array.Find(sounds, sound => sound.name == name);
        if (d == null)
            return;

        d.source.Stop();
    }
    public void StopAllSounds()
    {
        foreach (Sound s in sounds)
        {
            s.source.Stop();
        }
    }
    public bool IsPlayingSound(string name)
    {
        Sound d = Array.Find(sounds, sound => sound.name == name);
        return d.source.isPlaying;
    }


    public static AudioSource GetSound(string name)
    {
        return Instance.GetSoundByName(name);
    }

    public static void Play(string name)
    {
        try
        {
             Instance.PlaySound(name);

        }
        catch (NullReferenceException)
        {
            Debug.LogWarning("Entering game from level scene will lead to loss of Audio and is not reccomended!");
        }

    }

    public static void ChangeVolume(string name, float volume)
    {
        Instance.ChangeSoundVolume(name, volume);
    }

    public static void Stop(string name)
    {
        try
        {
            Instance.StopSound(name);
        }
        catch (NullReferenceException)
        {
            Debug.LogWarning("Entering game from level scene will lead to loss of Audio and is not reccomended!");
        }
    }
    public static void StopAll()
    {
        try
        {
            Instance.StopAllSounds();
        }
        catch (NullReferenceException)
        {
            Debug.LogWarning("Entering game from level scene will lead to loss of Audio and is not reccomended!");
        }
    }

    public static bool IsPlaying(string name)
    {
        try
        {
            return Instance.IsPlayingSound(name);
        }
        catch(NullReferenceException)
        {
            Debug.LogWarning("Entering game from level scene will lead to loss of Audio and is not reccomended!");
        }
        return false;
    }

    public static void Pause(string name)
    {
        try
        {
            Instance.PauseSound(name);

        }
        catch (NullReferenceException)
        {
            Debug.LogWarning("Entering game from level scene will lead to loss of Audio and is not reccomended!");
        }

    }

    public static void UnPause(string name)
    {
        try
        {
            Instance.UnPauseSound(name);

        }
        catch (NullReferenceException)
        {
            Debug.LogWarning("Entering game from level scene will lead to loss of Audio and is not reccomended!");
        }

    }

    public static void PauseAll()
    {
        pausedSounds.Clear(); // Resetãm lista
        foreach (Sound s in Instance.sounds)
        {
            if (s.source.isPlaying)
            {
                s.source.Pause();
                pausedSounds.Add(s.name);
            }
        }
    }

    public static void UnPauseAll()
    {
        foreach (string soundName in pausedSounds)
        {
            Sound s = Array.Find(Instance.sounds, sound => sound.name == soundName);
            if (s != null)
            {
                s.source.UnPause();
            }
        }
        pausedSounds.Clear();
    }
}
