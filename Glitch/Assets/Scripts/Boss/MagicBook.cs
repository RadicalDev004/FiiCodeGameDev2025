using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class MagicBook : MonoBehaviour
{
    public GameObject Eye;
    public bool isLookingAway = false, StopLookingAway = false;
    public float LookSpeed = 2f, LookAwaySpeed = 0.5f;
    public float InitialRotY, InitialRotX;
    public GameObject Pupil;
    public Material AngryEye;

    private void Awake()
    {
        InitialRotX = Eye.transform.eulerAngles.x;
        InitialRotY = Eye.transform.eulerAngles.y;
    }


    void LateUpdate()
    {
        Vector3 toEye = Eye.transform.position - Ref.PlayerBehaviour.playerCamera.transform.position;
        float angleToEye = Vector3.Angle(Ref.PlayerBehaviour.playerCamera.transform.forward, toEye);

        if(StopLookingAway || angleToEye > 3)
        {
            isLookingAway = false;
            Vector3 directionToPlayer = Ref.PlayerBehaviour.transform.position - Eye.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            Vector3 targetEuler = targetRotation.eulerAngles;


            Vector3 euler = targetEuler;

            euler.x = NormalizeAngle(euler.x);
            //euler.y = NormalizeAngle(euler.y);

            euler.x = Mathf.Clamp(euler.x, -40f, 40f);
            euler.y = Mathf.Clamp(euler.y, InitialRotY - 40f, InitialRotY + 40f);

            Quaternion clampedRotation = Quaternion.Euler(euler);
            Eye.transform.rotation = Quaternion.Slerp(Eye.transform.rotation, clampedRotation, Time.deltaTime * LookSpeed);

        }
        else if(!isLookingAway && !StopLookingAway)
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

    public void GetAngry()
    {
        Debug.Log("Angry boss");
        
        var renderer = Pupil.GetComponent<MeshRenderer>();
        var mats = renderer.materials;
        mats[0] = AngryEye;
        renderer.materials = mats;
    }
}
