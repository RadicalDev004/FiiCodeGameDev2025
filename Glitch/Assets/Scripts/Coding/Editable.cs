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
    [TextArea(5, 5)]
    public string HintText;
    public Func<List<string>, bool> ValidateCode;
    public bool Completed = false, Block = false;
    public bool UseOutline = true;
    public int[] EnemySpawnAfterComplete;

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

        Time.timeScale = 0;
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
        return newCode.Count > ind ? newCode[ind++] : null;
    }

    protected void OnGlitchSolve()
    {
        StartCoroutine(delayEnemies());
        playerBehaviour.PlaySolveGlitch();
        ToggleOutline(false);
        Completed = true;
    }

    private IEnumerator delayEnemies()
    {
        yield return new WaitForSeconds(2f);
        Ref.EnemySpawner.SpawnEnemies(0.5f, EnemySpawnAfterComplete);
    }
}