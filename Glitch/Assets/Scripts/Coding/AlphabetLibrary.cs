using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class AlphabetLibrary : Editable
{
    [Header("Particular")]
    public List<GameObject> Books = new();
    public List<string> CorrectAnswer = new();

    public Animator animatorCarti;

    private void Awake()
    {
        ValidateCode = Validate;
    }

    public bool Validate(List<string> code)
    {
        if (code.Count != CorrectAnswer.Count)
        {
            Debug.LogError("Failed validation at length " + code.Count);
            return false;
        }

        for(int i = 0; i < code.Count; i++)
        {
            code[i] = code[i].ToUpper();
            if (code[i] == CorrectAnswer[i])
            {
                Debug.Log("Correct at book " + i);
                Books[i].GetComponentInChildren<TMP_Text>().text = CorrectAnswer[i];
            }
        }

        if(code.SequenceEqual(CorrectAnswer))
        {
            StartCoroutine(WaitForWin(3));
        }

        return true;
    }

    private IEnumerator WaitForWin(float wait)
    {
        ToggleOutline(false);
        animatorCarti.Play("AnimatieCarti");

        yield return new WaitForSeconds(wait);

        OnGlitchSolve();
    }
}
