using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JarsColoring : Editable
{
    [Header("Particular")]
    public GameObject Jar1;
    public GameObject Jar2;
    public float FillJar1, FillJar2;
    public float MaxPos = -0.02f, MaxFill = 0.09f;
    public SpriteRenderer Org, ToEdit;

    private void Awake()
    {
        ValidateCode = Validate;
        StartCoroutine(JarsAnimation(0));
    }

    public bool Validate(List<string> code)
    {
        if (code.Count != 2)
        {
            Debug.LogError("Failed validation at length " + code.Count);
            return false;
        }

        List<float> values = new();
        for (int i = 0; i < 2; i++)
        {
            if (float.TryParse(code[i], out float value))
            {
                values.Add(value);
            }
            else
            {
                Debug.LogError("Failed validation at parse " + code[i]);
                return false;
            }
        }

        if (0 > values[0] || values[0] > 255)
        {
            Debug.LogError("Failed validation at incorrect value " + values[0]);
            return false;
        }
        if (0 > values[1] || values[1] > 255)
        {
            Debug.LogError("Failed validation at incorrect value " + values[1]);
            return false;
        }

        FillJar1 = values[0];
        FillJar2 = values[1];

        StartCoroutine(JarsAnimation(2));

        return true;
    }

    private IEnumerator JarsAnimation(float seconds)
    {
        Debug.Log("Old Color: " + ToEdit.color);
        Debug.Log("New color " + new Color(FillJar2, 0, FillJar1, 255));
        Tween.Color(ToEdit, new Color32((byte)FillJar2, 0, (byte)FillJar1, 255), seconds, 0, Tween.EaseInOut);
        EditLiquid(Jar1, FillJar1 / 255 * 100, seconds);
        EditLiquid(Jar2, FillJar2 / 255 * 100, seconds);
        yield return new WaitForSeconds(seconds);


        if((Org.color.b - 0.1f <= ToEdit.color.b && ToEdit.color.b <= Org.color.b + 0.1f) && (Org.color.r - 0.1f <= ToEdit.color.r && ToEdit.color.r <= Org.color.r + 0.1f))
        {
            OnGlitchSolve();
        }
    }

    public void EditLiquid(GameObject jar, float percent, float seconds)
    {
        float newScale = MaxFill * percent / 100;
        newScale = newScale > 0.01f ? newScale : 0.01f;
        float newPos = MaxPos - MaxFill + newScale;        
        
        Debug.LogWarning(percent + " " + newScale + " " + newPos);
        Tween.LocalScale(jar.transform, new Vector3(jar.transform.localScale.x, newScale, jar.transform.localScale.z), seconds, 0, Tween.EaseInOut);
        Tween.LocalPosition(jar.transform, new Vector3(jar.transform.localPosition.x, newPos, jar.transform.localPosition.z), seconds, 0, Tween.EaseInOut);
    }
}
