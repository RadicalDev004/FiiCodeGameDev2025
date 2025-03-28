using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AlphabetLibrary : Editable
{
    public string CorrectRaspuns = "BSDAML";

    public Animator animatorCarti;

    public float wait;

    private void Awake()
    {
        ValidateCode = Validate;
    }

    public bool Validate(List<string> code)
    {
        if (code.Count != 1)
        {
            Debug.LogError("Failed validation at length " + code.Count);
            return false;
        }

        code[0] = code[0].ToUpper();

        if (code[0].SequenceEqual(CorrectRaspuns))
        {
            StartCoroutine(WaitForWin());
            return true;
        }
        else
        {
            Debug.Log("String introdus: " + code[0]);
            return false;
        }
    }

    private IEnumerator WaitForWin()
    {
        ToggleOutline(false);
        animatorCarti.Play("AnimatieCarti");

        yield return new WaitForSeconds(wait);

        OnGlitchSolve();
    }
}
