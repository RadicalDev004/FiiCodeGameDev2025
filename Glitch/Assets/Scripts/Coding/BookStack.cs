using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BookStack : Editable
{
    [Header("Particular")]
    public List<GameObject> books = new();
    public List<Vector3> initialBookPos = new();    
    public List<Vector3> IntermediatePos = new();
    public List<int> CorrectAnswer = new();

    private void Awake()
    {
        ValidateCode = Validate;
    }
    public bool Validate(List<string> code)
    {
        if(code.Count != CorrectAnswer.Count)
        {
            Debug.LogError("Failed validation at length " + code.Count);
            return false;
        }

        List<int> values = new();
        foreach (string s in code)
        {
            if (int.TryParse(s, out int value))
            {
                values.Add(value);
            }
            else
            {
                Debug.LogError("Failed validation at parse " + s);
                return false;
            }
        }

        bool hasDuplicates = values.Count != values.Distinct().Count();
        if (hasDuplicates)
        {
            Debug.LogError("Failed validation at duplicates");
            return false;
        }

        for (int i = 0; i < values.Count; i++)
        {
            if (values[i] < 0 || values[i] >= books.Count)
            {
                Debug.LogError("Failed validation at incorrect value " + values[i]);
                return false; 
            }
        }

        StartCoroutine(BookPosAnimation(values));

        return true;
    }

    private IEnumerator BookPosAnimation(List<int> values)
    {
        if (values.SequenceEqual(CorrectAnswer))
        {
            Debug.Log("CORRECT!!!");
            OnGlitchSolve();
            
        }

        for (int i = 0; i < books.Count; i++)
        {
            Tween.Position(books[i].transform, IntermediatePos[i], 0.5f, 0, Tween.EaseInOut);
        }
        yield return new WaitForSeconds(2);

        for (int i = 0; i < values.Count; i++)
        {
           Tween.Position( books[i].transform, initialBookPos[values[i]], 0.5f, 0, Tween.EaseInOut );
        }        
    }
}
