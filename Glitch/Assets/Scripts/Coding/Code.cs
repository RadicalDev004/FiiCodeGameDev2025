using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Code : MonoBehaviour
{
    [TextArea(10,10)]
    public string ExecutableCode;
    public Editable CurrentEditable;

    //public TMP_Text T_Code;
    public TMP_Text T_ErrText;
    public TMP_InputField In_Editable;
    public TMP_Text T_Hint;
    public Button B_Close;
    public Button B_Reset;
    public static bool IsOpen = false;

    public int EditableCount = 0;

    public List<string> ExistingCode = new();
    public List<string> AddedCode = new();

    private Func<List<string>, bool> Validate;

    public delegate void ValidateCode(List<string> newCode);
    public static event ValidateCode OnValidate;

    private void Awake()
    {
        B_Close.onClick.AddListener(delegate
        {
            Close();
            OnValidate = null;
        });
        B_Reset.onClick.AddListener(delegate
        {
            
            Create(CurrentEditable);
        });
    }

    public void Create(Editable CurrEditable)
    {

        Debug.Log("Creating Code Environment");
        IsOpen = true;

        CurrentEditable = CurrEditable;
        In_Editable.text = string.Empty;
        ExecutableCode = CurrEditable.ExecutableCode;
        ExistingCode = new();
        AddedCode = new();
        Validate = CurrentEditable.ValidateCode;

        string pattern = @"<e>(.*?)</e>";

        MatchCollection matches = Regex.Matches(ExecutableCode, pattern);

        int LastIndex = 0;

        foreach (Match match in matches.Cast<Match>())
        {
            int charIndex = match.Index;

            string editable = match.Groups[1].Value;
            string existing = ExecutableCode[LastIndex..charIndex];

            ExistingCode.Add(existing);
            AddedCode.Add(editable);

            In_Editable.text += existing;
            In_Editable.text += editable;

            LastIndex = charIndex + editable.Length + 7;
        }

        ExistingCode.Add(ExecutableCode[LastIndex..(ExecutableCode.Length - 1)]);
        In_Editable.text += ExecutableCode[LastIndex..(ExecutableCode.Length - 1)];
        T_Hint.text = CurrEditable.HintText;
    }

    public void RunCode()
    {       
        List<string> edited = ExtractEditedCode(In_Editable.text.ToLower());
        //Debug.LogWarning("Running code with " + string.Join(", ", edited));
        if (edited == null) T_ErrText.text = "Compiler Error!";
        else if(!Validate(edited)) T_ErrText.text = "Validation Error!";
        else
        {
            Close();
            T_ErrText.text = string.Empty;
            OnValidate?.Invoke(edited);
            return;
        }

        StartCoroutine(ResetErrtext(2));
    }

    public void Close()
    {
        AudioManager.Play("Code_Close");

        gameObject.SetActive(false);
        LookPC.isPaused = false;
        Movement.IsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        IsOpen = false;
    }

    public List<string> ExtractEditedCode(string editedText)
    {
        List<string> newAddedCode = new();
        int currentIndex = 0;

        editedText = Regex.Replace(editedText, @"\s+", "");
        editedText = editedText.ToLower();

        foreach (string existing in ExistingCode)
        {
            string existingL = Regex.Replace(existing, @"\s+", "");

            int pos = editedText.IndexOf(existingL, currentIndex);
            if (pos == -1)
            {
                //Debug.LogError("<noparse>Compiler error at " + existingL.Replace("<", "＜").Replace(">", "＞") + " | " + editedText[currentIndex..].Replace("<", "＜").Replace(">", "＞") + " | " + editedText.IndexOf(existingL, currentIndex) + "</noparse>");
                return null;
            }

            string editedSegment = editedText[currentIndex..pos];
            if(editedSegment != string.Empty) newAddedCode.Add(editedSegment);

            currentIndex = pos + existingL.Length;
        }

        if(editedText[currentIndex..] != string.Empty) newAddedCode.Add(editedText[currentIndex..]);

        return newAddedCode;
    }

    private IEnumerator ResetErrtext(float f)
    {
        yield return new WaitForSecondsRealtime(f);
        T_ErrText.text = string.Empty;
    }

    public static bool HasAtMostOneDifference<T>(List<T> list1, List<T> list2)
    {
        if (list1.Count != list2.Count) return false;

        int diffCount = 0;

        for (int i = 0; i < list1.Count; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(list1[i], list2[i]))
            {
                diffCount++;
                if (diffCount > 1) return false;
            }
        }

        return true;
    }
}
