using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public GameObject Tab_Settings;
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
        Tab_Settings.SetActive(state);

        if (!Code.IsOpen && !MapManager.IsOpen)
        {
            LookPC.isPaused = state;
            Movement.IsPaused = state;
            Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
            Time.timeScale = state ? 0 : 1;
        }       
    }
}