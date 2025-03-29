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
    public List<float> CorrectDurations = new();

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
            if (duration < 0 || duration > 10)
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

        if (durations.SequenceEqual(CorrectDurations))
        {
            OnGlitchSolve();
            //foreach (var candle in candleRenderers)
            //{
            //    candle.material = CorrectMaterial;
            //}
        }
        else
        {
            //idk
        }

        Block = false;
    }

    //private void ResetCandles(List<Vector3> initialScales, List<MeshRenderer> candleRenderers)
    //{
    //    for (int i = 0; i < CandlesObj.Count; i++)
    //    {
    //        CandlesObj[i].transform.localScale = initialScales[i];
    //        candleRenderers[i].materials[0] = FireMaterial;
    //    }
    //}
}
