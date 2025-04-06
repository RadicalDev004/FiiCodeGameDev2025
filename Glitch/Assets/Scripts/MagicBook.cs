using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class MagicBook : MonoBehaviour
{
    public GameObject Eye;
    private bool isLookingAway = false;
    public float LookSpeed = 2f, LookAwaySpeed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 toEye = Eye.transform.position - Ref.PlayerBehaviour.playerCamera.transform.position;
        float angleToEye = Vector3.Angle(Ref.PlayerBehaviour.playerCamera.transform.forward, toEye);

        if(angleToEye > 3)
        {
            isLookingAway = false;
            Vector3 directionToPlayer = Ref.PlayerBehaviour.transform.position - Eye.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            Vector3 targetEuler = targetRotation.eulerAngles;


            Vector3 euler = Eye.transform.eulerAngles;

            euler.x = NormalizeAngle(euler.x);
            euler.y = NormalizeAngle(euler.y);

            euler.x = Mathf.Clamp(euler.x, -40f, 40f);
            euler.y = Mathf.Clamp(euler.y, -40f, 40f);

            Quaternion clampedRotation = Quaternion.Euler(targetEuler);
            Eye.transform.rotation = Quaternion.Slerp(Eye.transform.rotation, clampedRotation, Time.deltaTime * LookSpeed);
        }
        else if(!isLookingAway)
        {
            isLookingAway = true;
            LookAway();
        }
        
    }

    float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }

    void LookAway()
    {
        int cadran = 0;
        var currRot = Eye.transform.localEulerAngles;
        currRot.x = NormalizeAngle(currRot.x);
        currRot.y = NormalizeAngle(currRot.y);

        if (currRot.x >= 0 && currRot.y < 0)
            cadran = 0;
        else if (currRot.x < 0 && currRot.y < 0)
            cadran = 1;
        else if (currRot.x < 0 && currRot.y >= 0)
            cadran = 2;
        else if (currRot.x >= 0 && currRot.y >= 0)
            cadran = 3;

        Vector3 rot = cadran switch
        {
            0 => new(-40, 40, 0),
            1 =>new(40, 40, 0),
            2=>new(40, -40, 0),
            3=>new(-40,-40, 0),
            _=>new(0,0,0)
        };

        Debug.Log(currRot + " " + cadran);

        Tween.LocalRotation(Eye.transform, rot, LookAwaySpeed, 0, Tween.EaseOutStrong);
    }
}
