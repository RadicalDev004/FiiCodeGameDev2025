using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState : MonoBehaviour
{
    public GameObject Character, ShieldObj;
    public Movement Movement;
    public int Dir;
    public static bool RUN = false;
    public Vector2 Direction = new(0,0);
    public GameObject Item_Sword, Item_Shield;
    public static bool ShieldActive = false;

    private string LatestAttack;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {     

        if (Movement.IsPaused) return;


    }

    public void SetEuler()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Direction.y = 1;
            if (Input.GetKey(KeyCode.S))
                Direction.y = 0;
        }
        if (Input.GetKeyDown(KeyCode.D))
        { 
            Direction.x = 1;
            if (Input.GetKey(KeyCode.A))
                Direction.x = 0;
        }
        if (Input.GetKeyDown(KeyCode.S))
        { 
            Direction.y = -1;
            if (Input.GetKey(KeyCode.W))
                Direction.y = 0;
        }
        if (Input.GetKeyDown(KeyCode.A))
        { 
            Direction.x = -1;
            if (Input.GetKey(KeyCode.D))
                Direction.x = 0;
        }

        if(Input.GetKeyUp(KeyCode.W))
        {
            Direction.y = 0;
            if (Input.GetKey(KeyCode.S))
                Direction.y = -1;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            Direction.x = 0;
            if (Input.GetKey(KeyCode.A))
                Direction.x = -1;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            Direction.y = 0;
            if (Input.GetKey(KeyCode.W))
                Direction.y = 1;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            Direction.x = 0;
            if (Input.GetKey(KeyCode.D))
                Direction.x = 1;
        }
        TurnCharacter();
    }

    public void TurnCharacter()
    {
        Vector3 turnTo = new();

        if(Direction.y == 1)
        {
            turnTo.y = 0;
            if (Direction.x == 1)
                turnTo.y = 45;
            if (Direction.x == -1)
                turnTo.y = 315;
        }
        else if (Direction.y == -1)
        {
            turnTo.y = 180;
            if (Direction.x == 1)
                turnTo.y = 135;
            if (Direction.x == -1)
                turnTo.y = 225;
        }
        else if (Direction.x == 1)
        {
            turnTo.y = 90;
        }
        else if (Direction.x == -1)
        {
            turnTo.y = 270;
        }

        if (turnTo.y - Character.transform.eulerAngles.y > 180)
            turnTo.y -= 360;
        else if (Character.transform.eulerAngles.y - turnTo.y > 180)
            turnTo.y += 360;

        //Character.transform.eulerAngles = Vector3.Lerp(Character.transform.eulerAngles, turnTo, 0.2f);
    }

    
}
