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
    }

    public void RunCode()
    {
        List<string> edited = ExtractEditedCode(In_Editable.text.ToLower());
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


        foreach (string existing in ExistingCode)
        {
            string existingL = Regex.Replace(existing, @"\s+", "");

            int pos = editedText.IndexOf(existingL, currentIndex);
            if (pos == -1)
            {
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
}
