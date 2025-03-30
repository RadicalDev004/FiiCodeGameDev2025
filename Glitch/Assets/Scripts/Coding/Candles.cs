using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Candles : Editable
{
    [Header("Particular")]
    public List<ParticleSystem> CandlesParticles = new();
    public List<float> BurnRates = new();

    private void Awake()
    {
        ValidateCode = Validate;
    }

    public bool Validate(List<string> code)
    {
        if (code.Count != CandlesParticles.Count)
        {
            Debug.LogError("Validation failed: Incorrect number of inputs.");
            return false;
        }

        List<float> enteredDurations = new();

        foreach (string input in code)
        {   

            if (!float.TryParse(input, out float duration) || duration <= 0)
            {
                Debug.LogError("Validation failed: Invalid input - " + input);
                return false;

            }
            if (duration < 0 || duration > 20)
            {
                Debug.LogError("Failed validation at incorrect value " + duration);
                return false;
            }
            enteredDurations.Add(duration);

        }

        StartCoroutine(BurnCandles(enteredDurations));
        return true;
    }

    private IEnumerator BurnCandles(List<float> durations)
    {
        Block = true;
        List<float> remainingTime = new(durations);

        for (int i = 0; i < CandlesParticles.Count; i++)
        {
            CandlesParticles[i].Play();
        }


        while (remainingTime.Max() > 0)
        {
            for (int i = 0; i < CandlesParticles.Count; i++)
            {
                if (remainingTime[i] > 0)
                {
                    remainingTime[i] -= Time.deltaTime * BurnRates[i];
                   
                    if (remainingTime[i] <= 0)
                    {
                        CandlesParticles[i].Stop();
                    }
                }
            }
            yield return null;
        }

        yield return new WaitForSeconds(1);


        if (checkValid(durations))
            OnGlitchSolve();

        Block = false;
    }

    private bool checkValid(List<float> durations)
    {
        float div = durations[0] / BurnRates[0];
        if (div == durations[1] / BurnRates[1] &&
            div == durations[2] / BurnRates[2] &&
            div == durations[3] / BurnRates[3])
            return true;

        return false;
    }
}
