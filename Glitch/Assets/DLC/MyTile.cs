using System.Collections;
using System.Collections.Generic;
using Pixelplacement;
using UnityEngine;

public class MyTile : MonoBehaviour
{
    public int X, Y;
    public int Type, Rotation;
    public List<GameObject> pipes = new(); //0 empty, 1-4 pipe types, 5 start, 6 end.

    public void Create(Vector3 pos, int x, int y, int type = 0, int rot = 0)
    {
        X = x;
       this.Y = y;
        Type = type;
        transform.localPosition = pos;
        transform.eulerAngles = new Vector3(rot, transform.eulerAngles.y, 0);
        Rotation = rot;
        if(type != 0) pipes[type-1].SetActive(true);
    }

    public void RotateTile()
    {
        Debug.Log("Rotated " + X + " , " + Y);
        Tween.Rotate(transform, new Vector3(90, 0, 0), Space.Self, 1, 0, Tween.EaseInOut);
        Rotation = (Rotation + 90) % 360;
    }


}
