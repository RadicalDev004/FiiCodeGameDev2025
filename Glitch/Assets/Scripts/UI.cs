using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public GameObject I_PressE, I_NotEnoughMana;

    public void TogglePressE(bool state)
    {
        I_PressE.SetActive(state);
    }
    public void ToggleNotEnughMana(bool state)
    {
        I_NotEnoughMana.SetActive(state);
    }
    
    public void ToggleMap()
    {

    }
}
