using Pixelplacement;
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

    private void Start()
    {
        AudioManager.UpdateVolume();
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
        if (state)
            Tab_Settings.alpha = 1;
        else
            StartCoroutine(ResetSettingsAlphaAfterCloseCor(0.2f));

        Tab_Settings.interactable = state;
        Tab_Settings.blocksRaycasts = state;

        Tab_Settings.transform.GetChild(0).localScale = Vector3.one * (state ? 0 : 1);
        Tween.LocalScale(Tab_Settings.transform.GetChild(0), Vector3.one * (state  ? 1 : 0), 0.2f, 0, Tween.EaseInOut);

        if (!Code.IsOpen && !MapManager.IsOpen)
        {
            StartCoroutine(ResetAvailabilityAlphaAfterCloseCor(0.2f));
        }       
    }

    private IEnumerator ResetSettingsAlphaAfterCloseCor(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        Tab_Settings.alpha = 0;
    }

    private IEnumerator ResetAvailabilityAlphaAfterCloseCor(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        LookPC.isPaused = IsOpen;
        Movement.IsPaused = IsOpen;
        Cursor.lockState = IsOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale = IsOpen ? 0 : 1;
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

    public void UpdateVolume()
    {
        PlayerPrefs.SetFloat("Volume", S_Volume.value);
        AudioManager.UpdateVolume();
    }
}