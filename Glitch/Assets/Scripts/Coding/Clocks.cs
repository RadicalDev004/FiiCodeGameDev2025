using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Clocks : Editable
{
    [Header("Clocks")]
    public List<GameObject> ClockHourHands = new();
    public List<GameObject> ClockMinuteHands = new();
    public List<int> HourValues = new() { 0, 0, 0 };
    public List<int> MinuteValues = new() { 0, 0, 0 };

    public List<float> Multipliers = new() { 1.0f, 1.5f, 2.0f };

    private void Awake()
    {
        ValidateCode = Validate;
    }

    public bool Validate(List<string> code)
    {
        if (code.Count != 2)
        {
            Debug.LogError("Invalid input length: Expected 2 values");
            return false;
        }

        List<int> parsedValues = new();
        foreach (string input in code)
        {
            if (!int.TryParse(input, out int value))
            {
                Debug.LogError("Invalid input: " + input);
                return false;
            }
            parsedValues.Add(value);
        }

        if (parsedValues[0] < 0 || parsedValues[0] > 12)
        {
            Debug.LogError("Invalid hour value: " + parsedValues[0] + " (must be between 0 and 12)");
            return false;
        }
        if (parsedValues[1] < 0 || parsedValues[1] > 60)
        {
            Debug.LogError("Invalid minute value: " + parsedValues[1] + " (must be between 0 and 60)");
            return false;
        }

        ApplyClockScaling(parsedValues[0], parsedValues[1]);
        StartCoroutine(AnimateClocks());
        return true;
    }

    private void ApplyClockScaling(int baseHour, int baseMinute)
    {
        for (int i = 0; i < 3; i++)
        {
            HourValues[i] = Mathf.RoundToInt((baseHour * Multipliers[i]) % 12);
            MinuteValues[i] = Mathf.RoundToInt((baseMinute * Multipliers[i]) % 60);
        }
    }

    private IEnumerator AnimateClocks()
    {
        Block = true;
        for (int i = 0; i < 3; i++)
        {
            Tween.LocalRotation(ClockHourHands[i].transform, Quaternion.Euler(0, -HourValues[i] * 30, 0), 1, 0, Tween.EaseInOut);
            Tween.LocalRotation(ClockMinuteHands[i].transform, Quaternion.Euler(0, -MinuteValues[i] * 6, 0), 1, 0, Tween.EaseInOut);
        }
        yield return new WaitForSeconds(1);

        if (IsPuzzleSolved())
        {
            OnGlitchSolve();
        }
        Block = false;
    }

    private bool IsPuzzleSolved()
    {
        return HourValues.Max() - HourValues.Min() <= 1 && MinuteValues.Max() - MinuteValues.Min() <= 1;
    }
}
