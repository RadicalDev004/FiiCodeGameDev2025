using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Look : MonoBehaviour
{
    public GameObject Player, Camera;
    public Transform Orientation;

    public float rotationSpeed;
    public Vector3 InputDir;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if(Movement.IsPaused) return;

        Vector3 dir = Player.transform.position - new Vector3(Camera.transform.position.x, Player.transform.position.y, Camera.transform.position.z);  
        Orientation.forward = dir.normalized;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDir = Orientation.forward * vertical + Orientation.right * horizontal;
        InputDir = inputDir;

        if(inputDir != Vector3.zero)
        {
            Player.transform.forward = Vector3.Slerp(Player.transform.forward, inputDir.normalized, rotationSpeed);
        }
    }
}
