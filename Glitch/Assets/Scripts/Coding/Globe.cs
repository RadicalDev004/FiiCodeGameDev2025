using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Globe : Editable
{
    [Header("Particular")]
    public List<float> CorrectAnswer = new();
    private void Awake()
    {
        ValidateCode = Validate;
    }

    public bool Validate(List<string> code)
    {
        Debug.Log(string.Join(", ", code));
        if (code.Count != 2)
        {
            Debug.LogError("Failed validation at length " + code.Count);
            return false;
        }

        List<float> values = new();
        foreach (string s in code)
        {
            string str = s.Replace(',', '.'); // Convert to dot format
            if (float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
            {
                Debug.LogWarning(value);
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

        if (-360 > values[0] || values[0] > 360)
        {
            Debug.LogError("Failed validation at incorrect value " + values[0]);
            return false;
        }

        if (0.1f > values[1] || values[1] > 2)
        {
            Debug.LogError("Failed validation at incorrect value " + values[1]);
            return false;
        }
        Debug.Log(values[0] + " " + values[1]);
        Tween.LocalScale(transform, Vector3.one * values[1], 2, 0, Tween.EaseInOut);
        Tween.LocalRotation(transform, Quaternion.Euler(0, values[0], 0), 2, 0, Tween.EaseInOut);

        StartCoroutine(WaitForWin(values, 2));

        return true;
    }

    private IEnumerator WaitForWin(List<float> values, float wait)
    {
        yield return new WaitForSeconds(wait);

        if (CorrectAnswer[0] <= values[0] && values[0] <= CorrectAnswer[1] && CorrectAnswer[2] <= values[1] && values[1] <= CorrectAnswer[3])
        {
            OnGlitchSolve();
        }
    }
}
