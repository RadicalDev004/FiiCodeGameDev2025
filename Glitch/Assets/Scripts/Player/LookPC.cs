using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookPC : MonoBehaviour
{
    public float sensitivity = 100f;
    public static bool isPaused = false;
    public float yRot, XRot;
    public float x, y;

    public GameObject Camera;

    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;

        Camera.transform.eulerAngles = new Vector3(0, 180, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        sensitivity = 5 + PlayerPrefs.GetFloat("Sensitivity") * 5;

        if (isPaused == false)
        {
            yRot -= Input.GetAxis("Mouse Y") * sensitivity;
            XRot += Input.GetAxis("Mouse X") * sensitivity;

            yRot = Mathf.Clamp(yRot, -90f, 90f);

            Camera.transform.eulerAngles = new Vector3 (yRot, XRot, 0);
        }       
    }
}
