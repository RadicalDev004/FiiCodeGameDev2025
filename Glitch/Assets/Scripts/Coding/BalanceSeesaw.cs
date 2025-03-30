using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BalanceSeesaw : Editable
{
    [Header("Balancing Setup")]
    public List<GameObject> Objects = new();
    public List<int> Weights = new();
    public Transform[] LeftPositions, RightPositions;
    public Transform BalancePivot;
    public float RotationSpeed = 0.5f; // Timp de tweening
    public float SwapSpeed = 0.5f;

    private Dictionary<GameObject, int> objectValues = new Dictionary<GameObject, int>();
    private Dictionary<string, GameObject> objectNames = new Dictionary<string, GameObject>();

    private void Awake()
    {
        ValidateCode = Validate;
        SetWeights();
    }

    void SetWeights()
    {
        for (int i = 0; i < Objects.Count; i++)
        {
            objectValues[Objects[i]] = Weights[i];

            string name = Objects[i].name.ToLower();
            objectNames[name] = Objects[i];
        }
    }

    public bool Validate(List<string> code)
    {
        if (code.Count != 2)
        {
            Debug.LogError("Validation failed: Incorrect number of inputs.");
            return false;
        }

        string name1 = code[0].ToLower();
        string name2 = code[1].ToLower();

        if (!objectNames.ContainsKey(name1) || !objectNames.ContainsKey(name2))
        {
            Debug.LogError("Validation failed: One or both object names are invalid.");
            return false;
        }

        GameObject obj1 = objectNames[name1];
        GameObject obj2 = objectNames[name2];

        if (obj1 == obj2)
        {
            Debug.LogError("Validation failed: Cannot swap the same object.");
            return false;
        }

        StartCoroutine(SwapObjects(obj1, obj2));
        return true;
    }

    private IEnumerator SwapObjects(GameObject obj1, GameObject obj2)
    {
        Block = true;
        Transform parent1 = obj1.transform.parent;
        Transform parent2 = obj2.transform.parent;

        Vector3 worldPos1 = obj1.transform.position;
        Vector3 worldPos2 = obj2.transform.position;

        Tween.Position(obj1.transform, worldPos2, 1, 0, Tween.EaseInOut);
        Tween.Position(obj2.transform, worldPos1, 1, 0, Tween.EaseInOut);

        yield return new WaitForSeconds(1f);

        obj1.transform.SetParent(parent2, true);
        obj2.transform.SetParent(parent1, true);

        obj1.transform.localPosition = Vector3.zero;
        obj2.transform.localPosition = Vector3.zero;

        UpdateBalance();

        yield return new WaitForSeconds(1f);
        Block = false;
    }



    private void UpdateBalance()
    {
        float leftWeight = 0;
        float rightWeight = 0;

        for (int i = 0; i < LeftPositions.Length; i++)
        {
            leftWeight += objectValues[LeftPositions[i].GetChild(0).gameObject];
        }
        for (int i = 0; i < RightPositions.Length; i++)
        {
            rightWeight += objectValues[RightPositions[i].GetChild(0).gameObject];
        }

        Debug.Log("stanga: " + leftWeight + " | dreapta: " + rightWeight);

        float targetAngle = 0f;
        if (rightWeight > leftWeight)
            targetAngle = 6.67f;
        else if (leftWeight > rightWeight)
            targetAngle = -6.67f;

        StartCoroutine(RotateBalancePivot(targetAngle));
    }

    private IEnumerator RotateBalancePivot(float targetAngle)
    {
        float currentAngle = BalancePivot.rotation.eulerAngles.x;
        float elapsedTime = 0f;

        while (elapsedTime < RotationSpeed)
        {
            float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, elapsedTime / RotationSpeed);
            BalancePivot.rotation = Quaternion.Euler(newAngle, 17.7f, 0f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        BalancePivot.rotation = Quaternion.Euler(targetAngle, 17.7f, 0f);
        CheckSolution(targetAngle);
    }

    private void CheckSolution(float targetAngle)
    {
        if (targetAngle == 0f)
        {
            OnGlitchSolve();
        }
    }
}