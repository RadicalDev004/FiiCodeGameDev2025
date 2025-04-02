using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public CanvasGroup Tab_Settings;
    public Slider S_Volume;
    public BoolSlider BS_Arrow, BS_EnemyRadar;

    public static bool IsOpen = false;


    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            ToggleSettings(true);
        }
    }

    public void ToggleSettings(bool state)
    {
        IsOpen = state;
        Tab_Settings.alpha = state ? 1 : 0;
        Tab_Settings.interactable = state;
        Tab_Settings.blocksRaycasts = state;

        if (!Code.IsOpen && !MapManager.IsOpen)
        {
            LookPC.isPaused = state;
            Movement.IsPaused = state;
            Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
            Time.timeScale = state ? 0 : 1;
        }       
    }

    private void OnEnable()
    {
        BS_Arrow.OnPress += ManageArrow;
        BS_EnemyRadar.OnPress += ManageEnemyRadar;

        S_Volume.value = PlayerPrefs.GetFloat("Volume");
        AudioManager.UpdateVolume();
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

    public void UpdateVolume()
    {
        PlayerPrefs.SetFloat("Volume", S_Volume.value);
        AudioManager.UpdateVolume();
    }
}