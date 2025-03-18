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

        if(UseOutline)
            Outline = GetComponent<Outline>();
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

    public void ToggleOutline(bool state)
    {
        if (!UseOutline || Outline == null) return;

        Outline.enabled = state;
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
        playerBehaviour.PlaySolveGlitch();
        ToggleOutline(false);
        Completed = true;
    }
}