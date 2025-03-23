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
        for(int i = 0; i < 3; i++)
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

        if (values[0] < 0.5f)
        {
            Debug.LogError("Failed validation at incorrect value " + values[0]);
            return false;
        }
        if (values[1] < 0.5f)
        {
            Debug.LogError("Failed validation at incorrect value " + values[1]);
            return false;
        }
        if (values[2] < 0.5f)
        {
            Debug.LogError("Failed validation at incorrect value " + values[2]);
            return false;
        }

        if (code[3] != "true" && code[3] != "false")
        {
            Debug.LogError("Failed validation at type " + code[3]);
            return false;
        }

        EnemyBehaviour.Create(EnemyBehaviour.MaxHealth, values[0], values[1], values[2], bool.Parse(code[3]), EnemyBehaviour.Difficulty, false);


        ManaSystem.Instance.UseMana(1);
        return true;
    }





}
