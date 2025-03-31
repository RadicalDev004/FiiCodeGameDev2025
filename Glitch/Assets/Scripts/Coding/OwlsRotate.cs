using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OwlsRotate : Editable
{
    [Header("Particular")]
    public List<string> OwlNames = new() { "red", "blue", "green", "purpele" };
    public List<GameObject> Owls = new();
    public List<float> CorrectAnswers = new();
    public List<Material> Materials = new();

    private void Awake()
    {
        ValidateCode = Validate;
        SetUpEyes();
    }

    public bool Validate(List<string> code)
    {
        if (code.Count != 2)
        {
            Debug.LogError("Failed validation at length " + code.Count);
            return false;
        }

        if (!OwlNames.Contains(code[0]) || !OwlNames.Contains(code[1])) 
        {
            Debug.LogError("Failed validation at type " + code[0] + " " + code[1]);
            return false;
        }

        if (code[0] == code[1])
        {
            Debug.LogError("Failed validation at duplicate " + code[0] + " " + code[1]);
            return false;
        }

        StartCoroutine(OwlAnimations(2, Owls[OwlNames.IndexOf(code[0])], Owls[OwlNames.IndexOf(code[1])]));
        return true;
    }

    private IEnumerator OwlAnimations(float duration, GameObject o1, GameObject o2)
    {
        Block = true;
        Tween.LocalRotation(o1.transform, o1.transform.localRotation * Quaternion.Euler(0, 90, 0), duration, 0, Tween.EaseInOutStrong);
        Tween.LocalRotation(o2.transform, o2.transform.localRotation * Quaternion.Euler(0, 90, 0), duration, 0, Tween.EaseInOutStrong);
        yield return new WaitForSeconds(duration);

        List<float> currRot = new() { Owls[0].transform.localEulerAngles.y % 360, Owls[1].transform.localEulerAngles.y % 360, Owls[2].transform.localEulerAngles.y % 360, Owls[3].transform.localEulerAngles.y % 360 };

        SetUpEyes();

        if(CorrectAnswers.SequenceEqual(currRot))
        {
            OnGlitchSolve();
        }
        Block = false;
    }

    public void SetUpEyes()
    {
        Debug.Log("setting up owl eyes");
        List<float> currRot = new() { Owls[0].transform.localEulerAngles.y % 360, Owls[1].transform.localEulerAngles.y % 360, Owls[2].transform.localEulerAngles.y % 360, Owls[3].transform.localEulerAngles.y % 360 };
        for (int i = 0; i <= 3; i++)
        {
            if (currRot[i] == CorrectAnswers[i])
            {
                Owls[i].transform.GetChild(0).GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                Owls[i].transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Materials[i].GetColor("_Color") * 2.0f);
                Owls[i].transform.GetChild(1).GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                Owls[i].transform.GetChild(1).GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Materials[i].GetColor("_Color") * 2.0f);
            }
            else
            {
                Owls[i].transform.GetChild(0).GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
                Owls[i].transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.black);
                Owls[i].transform.GetChild(1).GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
                Owls[i].transform.GetChild(1).GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.black);
            }
        }
    }
}
