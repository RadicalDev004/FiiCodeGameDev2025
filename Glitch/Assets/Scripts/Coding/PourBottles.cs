using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PourBottles : Editable
{
    public static List<string> bottles = new() { "bottle_big", "bottle_medium", "bottle_small" };
    [Header("Particular")]
    
    public List<GameObject> B = new();

    public List<TransformList> PouringPos = new();
    public List<int> max = new(){ 8, 5, 3 };
    public List<int> curr = new() { 8, 0, 0 };
    public float MaxPos = -0.02f, MaxFill = 0.08f;

    private void Awake()
    {
        ValidateCode = Validate;
        for(int i = 0; i < bottles.Count; i++)
        {
            EditLiquid(B[i].transform.GetChild(0).gameObject, max[i], curr[i], 0);
        }
    }
    public bool Validate(List<string> code)
    {
        if (code.Count != 2)
        {
            Debug.LogError("Failed validation at length " + code.Count);
            return false;
        }
        if (!bottles.Contains(code[0]) || !bottles.Contains(code[1]))
        {
            Debug.LogError("Failed validation at type " + code[0] + " " + code[1]);
            return false;
        }
        if (code[0] == code[1])
        {
            Debug.LogError("Failed validation at duplicate " + code[0] + " " + code[1]);
            return false;
        }

        int index1 = bottles.IndexOf(code[0]);
        int index2 = bottles.IndexOf(code[1]);

        
        StartCoroutine(PouringAnimationCor(index1, index2, 1));
       

        

        return true;
    }

    private IEnumerator PouringAnimationCor(int index1, int index2, float time)
    {
        Block = true;

        GameObject b1 = B[index1];
        int m1 = max[index1];
        int curr1 = curr[index1];

        GameObject b2 = B[index2];
        int m2 = max[index2];
        int curr2 = curr[index2];

        int amToTransfer = Mathf.Min(curr1, m2 - curr2);

        List<int> arr = new(){ 3, 2, 1};
        arr.RemoveAt(index1);
        Dictionary<int, int> mapping = new();

        for (int i = 0; i < arr.Count; i++)
        {
            mapping[arr[i]] = i;
        }

        Translate(b1.transform, PouringPos[index1].transforms[mapping[3 - index2]], time);

        yield return new WaitForSeconds(time);
        EditLiquid(b1.transform.GetChild(0).gameObject, m1, curr1 - amToTransfer, time);
        EditLiquid(b2.transform.GetChild(0).gameObject, m2, curr2 + amToTransfer, time);

        yield return new WaitForSeconds(time);
        Translate(b1.transform, PouringPos[index1].transforms[2], time);
        yield return new WaitForSeconds(time);

        curr[index1] -= amToTransfer;
        curr[index2] += amToTransfer;

        if (curr[1] == 4)
        {
            OnGlitchSolve();
        }

        Block = false;
    }

    public void Translate(Transform t1, Transform t2, float time)
    {
        Tween.LocalPosition(t1, t2.localPosition, time, 0, Tween.EaseInOut);
        Tween.LocalRotation(t1, t2.localRotation, time, 0, Tween.EaseInOut);
    }

    public void EditLiquid(GameObject B, float max, float amount, float seconds)
    {
        float percent = amount * 100 / max;
        float newScale = MaxFill * percent / 100;
        newScale = newScale > 0.001f ? newScale : 0.001f;
        float newPos = MaxPos - MaxFill + newScale;

        Tween.LocalScale(B.transform, new Vector3(B.transform.localScale.x, newScale, B.transform.localScale.z), seconds, 0, Tween.EaseInOut);
        Tween.LocalPosition(B.transform, new Vector3(B.transform.localPosition.x, newPos, B.transform.localPosition.z), seconds, 0, Tween.EaseInOut);
    }
}

[System.Serializable]
public class TransformList
{
    public List<Transform> transforms = new();
}
