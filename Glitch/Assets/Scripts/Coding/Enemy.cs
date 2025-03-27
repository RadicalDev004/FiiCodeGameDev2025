using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Editable
{
    [Header("Particular")]
    public EnemyBehaviour EnemyBehaviour;

    private void Awake()
    {
        EnemyBehaviour = GetComponentInParent<EnemyBehaviour>();
        ValidateCode = Validate;
    }
    public bool Validate(List<string> code)
    {
        if (!Code.HasAtMostOneDifference(code, Ref.Code.AddedCode))
            return false;

        List<float> values = new();
        for(int i = 0; i < 4; i++)
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

        if ( 0.5f > values[0] || values[0] > 5)
        {
            Debug.LogError("Failed validation at incorrect value " + values[0]);
            return false;
        }
        if (1 > values[1] || values[1] > 100)
        {
            Debug.LogError("Failed validation at incorrect value " + values[1]);
            return false;
        }
        if (0.5f > values[2] || values[2] > 5)
        {
            Debug.LogError("Failed validation at incorrect value " + values[2]);
            return false;
        }
        if (0.5f > values[3] || values[3] > 5)
        {
            Debug.LogError("Failed validation at incorrect value " + values[3]);
            return false;
        }

        if (code[4] != "true" && code[4] != "false")
        {
            Debug.LogError("Failed validation at type " + code[4]);
            return false;
        }

        EnemyBehaviour.Create(EnemyBehaviour.MaxHealth, values[0], values[1], values[2], values[3], bool.Parse(code[4]), EnemyBehaviour.Difficulty, false);


        ManaSystem.Instance.UseMana(1);
        return true;
    }





}
