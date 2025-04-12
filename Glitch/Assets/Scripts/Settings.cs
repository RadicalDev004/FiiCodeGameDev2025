using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public CanvasGroup Tab_Settings;
    public Slider S_Volume, S_Effects;
    public BoolSlider BS_Arrow, BS_EnemyRadar;
    public bool isMainMenu = false;

    public static bool IsOpen = false;

    private void Start()
    {
        Debug.Log(Time.timeScale);
        Movement.isPaused = false;
        LookPC.isPaused = false;
        LoadVolume();
        LoadEffects();

        Debug.Log("am inceput");
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape) && !IsOpen)
        {
            ToggleSettings(true);
        }
    }

    public void ToggleSettings(bool state)
    {
        IsOpen = state;
        Tab_Settings.interactable = state;
        Tab_Settings.blocksRaycasts = state;

        Tab_Settings.transform.GetChild(0).localScale = Vector3.one * (state ? 0 : 1);
        Tween.LocalScale(Tab_Settings.transform.GetChild(0), Vector3.one * (state ? 1 : 0), 0.2f, 0, Tween.EaseInOut);

        if (state)
        { 
            Tab_Settings.alpha = 1;

            if(!isMainMenu)
            {
                Debug.Log("nu e main menu");
                AudioManager.PauseAll();
                LookPC.isPaused = true;
                Movement.isPaused = true;
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
            }

        }
        else
        {
            AudioManager.UnPauseAll();

            Ref.ActionAfterTime(0.2f, delegate { Tab_Settings.alpha = 0; });
            if (!Code.IsOpen && !MapManager.IsOpen && !isMainMenu)
            {
                Debug.Log("e main menu");
                Ref.ActionAfterTime(0.2f, delegate {
                    LookPC.isPaused = false;
                    Movement.isPaused = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    Time.timeScale = 1;
                });
            }
        }                    

             
    }


    private void OnEnable()
    {
        BS_Arrow.OnPress += ManageArrow;
        BS_EnemyRadar.OnPress += ManageEnemyRadar;       
    }
    private void OnDisable()
    {
        BS_Arrow.OnPress -= ManageArrow;
        BS_EnemyRadar.OnPress -= ManageEnemyRadar;
    }

    public void ManageArrow(bool state)
    {
        GuideArrow.Toggle = state;
    }

    public void ManageEnemyRadar(bool state)
    {
        EnemyRadar.isActive = state;
    }

    public void LoadVolume()
    {
        if (!PlayerPrefs.HasKey("Volume")) PlayerPrefs.SetFloat("Volume", 0.5f);
        S_Volume.value = PlayerPrefs.GetFloat("Volume");
        AudioManager.UpdateVolume();
    }

    public void UpdateVolume()
    {
        PlayerPrefs.SetFloat("Volume", S_Volume.value);
        AudioManager.UpdateVolume();
    }

    public void LoadEffects()
    {
        if (!PlayerPrefs.HasKey("SoundEffects")) PlayerPrefs.SetFloat("SoundEffects", 0.5f);
        S_Effects.value = PlayerPrefs.GetFloat("SoundEffects");
        AudioManager.UpdateVolume();
    }

    public void UpdateEffects()
    {
        PlayerPrefs.SetFloat("SoundEffects", S_Effects.value);
        AudioManager.UpdateVolume();
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        IsOpen = false;
    }
}