using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

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

        if (UseOutline)
            Outline = GetComponent<Outline>();
    }

    public void OpenTerminal()
    {
        if (MapManager.IsOpen || Settings.IsOpen) return;
        StartCoroutine(ToggleTerminal(true));
    }

    public void CloseTerminal()
    {
        StartCoroutine(ToggleTerminal(false));
    }

    public IEnumerator ToggleTerminal(bool open)
    {
        LookPC.isPaused = open;
        Movement.isPaused = open;
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;

        if (open)
        {
            AudioManager.Play("Code_Open");
            Cursor.lockState = CursorLockMode.None;
            Code.gameObject.SetActive(true);
            Code.transform.GetChild(0).localScale = Vector3.zero;
            Tween.LocalScale(Code.transform.GetChild(0), Vector3.one, 0.2f, 0f, Tween.EaseInOut);
            Code.Create(this);
            Time.timeScale = 0;
            Code.OnValidate += SaveCode;
            Code.IsOpen = true;
        }
        else
        {
            AudioManager.Play("Code_Close");
            Cursor.lockState = CursorLockMode.Locked;
            Tween.LocalScale(Code.transform.GetChild(0), Vector3.zero, 0.2f, 0f, Tween.EaseInOut);
            yield return new WaitForSecondsRealtime(0.2f);
            Code.gameObject.SetActive(false);
            Time.timeScale = 1;
            Code.IsOpen = false;
        }
    }

    private IEnumerator ScaleUI(Transform obj, Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            obj.localScale = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        obj.localScale = to;
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
        ExecutableCode = Regex.Replace(ExecutableCode, pattern, match => $"<e>{ReplaceValue(newCode, ref ind)}</e>");
    }

    static string ReplaceValue(List<string> newCode, ref int ind)
    {
        return newCode.Count > ind ? newCode[ind++] : null;
    }

    public void OnGlitchSolve(bool redo = false)
    {
        Debug.Log("ON GLITCH COMPLETE");
        AudioManager.Play("Puzzle_Solved");
        StartCoroutine(delayEnemies());
        playerBehaviour.PlaySolveGlitch();
        ToggleOutline(false);
        Completed = true;

        if(!redo)
        {
            PlayerBehaviour.GlitchesSolved++;
            Ref.PlayerBehaviour.StaffStone.material.SetFloat("_FresnelPower", 1 - 0.13f * PlayerBehaviour.GlitchesSolved);
            Ref.SaveSystem.SaveState();
            SaveSystem.LatestSolvedGlitch = this;
        }                
    }

    private IEnumerator delayEnemies()
    {
        yield return new WaitForSeconds(2f);
        Ref.EnemySpawner.SpawnEnemies(0.5f, EnemySpawnAfterComplete);
    }
}
