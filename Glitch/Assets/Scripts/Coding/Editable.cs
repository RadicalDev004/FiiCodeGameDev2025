using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class Editable : MonoBehaviour
{
    [Header("Editable")]
    [TextArea(15, 15)]
    public string ExecutableCode;
    public Func<List<string>, bool> ValidateCode;
    public bool Completed = false;
    public bool UseOutline = true;


    private PlayerBehaviour playerBehaviour;
    private Code Code;
    private UI UI;
    [HideInInspector]
    public Outline Outline;

    private void Start()
    {
        playerBehaviour = Ref.PlayerBehaviour;
        Code = Ref.Code;
        UI = Ref.UI;
        ExecutableCode = ExecutableCode.ToLower();
    }

    public void CreateTerminal()
    {
        LookPC.isPaused = true;
        Movement.IsPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Code.gameObject.SetActive(true);
        Code.Create(this);

        Code.OnValidate += SaveCode;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player" && !Completed)
        {
            Debug.Log("Player Enter Editable " + name);
            playerBehaviour.AddEditable(this);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player" && !Completed)
        {
            Debug.Log("Player Exited Editable " + name);
            ToggleOutline(false);
            playerBehaviour.RemoveEditable(this);
        }
    }

    public void ToggleOutline(bool state)
    {
        if (!UseOutline) return;

        if(state)
        {
            if (Outline != null) return;
            Outline = gameObject.AddComponent<Outline>();
            Outline.OutlineMode = Outline.Mode.OutlineVisible;
            Outline.OutlineWidth = 7;
        }
        else
        {
            Destroy(Outline);
        }
    }


    public void SaveCode(List<string> newCode)
    {
        Code.OnValidate -= SaveCode;

        string pattern = @"<e>(.*?)</e>";
        int ind = 0;

        string updatedText = Regex.Replace(ExecutableCode, pattern, match => $"<e>{ReplaceValue(newCode, ref ind)}</e>");
        ExecutableCode = updatedText;
    }

    static string ReplaceValue(List<string> newCode, ref int ind)
    {        
        return newCode[ind++];
    }

    protected void OnGlitchSolve()
    {
        playerBehaviour.RemoveEditable(this);
        playerBehaviour.ToggleSolveGlitch(true);
        ToggleOutline(false);
        Completed = true;
    }
}
