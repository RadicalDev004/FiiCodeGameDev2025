using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinManager : MonoBehaviour
{
    public GameObject Tab_Win;
    public TMP_Text T_TotalTime, T_AvarageTime;

    public DateTime StartTime;

    // Start is called before the first frame update
    void Start()
    {
        StartTime = DateTime.Now;
    }

    /*private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            Win();
        }
    }*/

    public void Win()
    {
        AudioManager.PauseAll();
        LookPC.isPaused = true;
        Movement.isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        Tab_Win.SetActive(true);
        Tween.LocalScale(Tab_Win.transform.GetChild(0), Vector3.one, 0.2f, 0, Tween.EaseInOut);

        var totalTime = DateTime.Now - StartTime;

        T_TotalTime.text = "Total time: " + totalTime.ToString(@"hh':'mm':'ss");

        Editable[] allPuzzles = FindObjectsOfType<Editable>();

        float sum = 0;
        int cnt = 0;

        foreach(var pzz in allPuzzles)
        {
            if (pzz.IgnoreTime || pzz.TimeSpent == 0) continue;
            sum += pzz.TimeSpent;
            cnt++;
        }

        var span = TimeSpan.FromSeconds(cnt > 0 ? sum / cnt : 0);

        T_AvarageTime.text = "Avarage time spent on a puzzle: " + span.ToString(@"hh':'mm':'ss");
    }
}
