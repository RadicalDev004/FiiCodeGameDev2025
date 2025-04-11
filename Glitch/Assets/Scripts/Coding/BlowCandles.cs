using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Windows;

public class BlowCandles : Editable
{
    [Header("Particular")]
    public List<ParticleSystem> Flames = new();
    public List<bool> States = new(3);

    private void Awake()
    {
        ValidateCode = Validate;
        SetCandlesAccordingToState();
    }
    
    public bool Validate(List<string> code)
    {
        if (code.Count > 1)
        {
            Debug.LogError("Failed validation at length " + code.Count);
            return false;
        }

        string input = code[0];
        string pattern = @"^candles\[(\d+)\]$";
        int index;

        Match match = Regex.Match(input, pattern);

        if (match.Success)
        {
            index = int.Parse(match.Groups[1].Value);
        }
        else
        {
            Debug.LogError("Failed validation at type " + code[0]);
            return false;
        }
        if (!States[index]) return true;

        States[index] = !States[index];
        switch(index)
        {
            case 0:
                if (!States[1] && !States[2]) { States[1] = true; States[2] = true; }
                if (States[1] && !States[2]) { States[1] = true; States[2] = false; }
                if (!States[1] && States[2]) { States[1] = true; States[2] = true; }
                break;


            case 1:
                if (!States[0] && States[2]) { States[0] = true; States[2] = true; }
                if (States[0] && !States[2]) { States[0] = false; States[2] = true; }
                if (!States[0] && !States[2]) { States[0] = false; States[2] = false; }
                break;


            case 2:
                if (States[0] && !States[1]) { States[0] = true; States[1] = false; }
                if (!States[0] && States[1]) { States[0] = true; States[1] = true; }
                if (!States[0] && !States[1]) { States[0] = true; States[1] = false; }
                break;

        }
        SetCandlesAccordingToState();
        Block = true;

        Ref.ActionAfterTime(1, delegate
        {
            Block = false;

            foreach (var st in States)
            {
                if (st) return;
            }

            OnGlitchSolve();
        });

        return true;
    }

    public void SetCandlesAccordingToState()
    {
        for(int i = 0; i < Flames.Count; i++)
        {
            if (States[i]) Flames[i].Play();
            else Flames[i].Stop();
        }
    }


}
