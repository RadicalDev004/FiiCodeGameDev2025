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
        ValidateCode = Validate;
    }
    public bool Validate(List<string> code)
    {
        

        return true;
    }





}
