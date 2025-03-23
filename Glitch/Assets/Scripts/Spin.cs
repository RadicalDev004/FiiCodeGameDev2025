using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public int Side;
    public float speed;

    private void Update()
    {
        transform.Rotate(SideToVectorType(), speed);
    }

    Vector3 SideToVectorType()
    {
        return Side switch
        {
            0 => Vector3.forward,
            1 => Vector3.up,
            2 => Vector3.right,
            _ => Vector3.zero,
        };
    }
}
