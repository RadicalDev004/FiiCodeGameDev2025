using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Mushroom : Editable
{
    public float Multiplier = 0;
    public int MaxCap = 10;
    [SerializeField] private float TimeOut = 0.2f;
    private bool Cooldwon = false;

    private void Awake()
    {
        ValidateCode = Validate;
    }

    public bool Validate(List<string> code)
    {
        if (code.Count != 1)
        {
            Debug.LogError("Failed validation at length " + code.Count);
            return false;
        }


        int value;
        if (int.TryParse(code[0], out int vl))
        {
            value = vl;
        }
        else
        {
            Debug.LogError("Failed validation at parse " + code[0]);
            return false;
        }

        if(value < 0 || value > MaxCap)
        {
            Debug.LogError("Failed validation at incorrect value " + value);
            return false;
        }

        Multiplier = (float)value/3;

        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            if(!Cooldwon) 
            {
                Ref.Movement.Jump(Multiplier);
                Cooldwon = true;
                StartCoroutine(WaitForCooldown(TimeOut));
            }
            
        }
    }

    private IEnumerator WaitForCooldown(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Cooldwon = false;
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            Ref.Movement.Jump(Multiplier);
        }
    }*/
}
