using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public GameObject I_PressE;

    public void TogglePressE(bool state)
    {
        I_PressE.SetActive(state);
    }
}
