using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MushroomTeleport : Editable
{
    public float Multiplier = 0;
    public int MaxCap = 10;
    public List<Vector3> teleportList = new List<Vector3>();

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

        if (value < 0 || value > MaxCap)
        {
            Debug.LogError("Failed validation at incorrect value " + value);
            return false;
        }

        Vector3 targetPosition = teleportList[value];
        GameObject player = Ref.PlayerBehaviour.gameObject;

        if(player.TryGetComponent(out CharacterController controller))
        {
            controller.enabled = false;
            player.transform.position = targetPosition;
            controller.enabled = true;
        }

        return true;
    }
}
