using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ManaSystem : MonoBehaviour
{
    public static ManaSystem Instance;
    public event Action<float> OnManaChanged;

    public float currentMana = 0;

    private float maxMana = 1;

    public static float ExtraManaPerHit;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        currentMana = maxMana;
    }

    public void AddMana(float amount)
    {
        if (BossCrystal.C5) return;

        if(currentMana < maxMana)
        {
            currentMana += amount + amount * ExtraManaPerHit * 25 / 100;
            OnManaChanged?.Invoke(currentMana);

            Debug.Log("current mana: " +  currentMana);
            if (currentMana > maxMana)
            {
                Debug.Log("mana max");
                OnManaChanged?.Invoke(currentMana);
                currentMana = maxMana;
            }
        }
    }

    public bool HasFullMana()
    {
        return currentMana == maxMana;
    }

    public void UseMana(float amount)
    {
        if(amount <= currentMana)
        {
            Debug.Log("Used " + amount + " mana");
            currentMana -= amount;
            OnManaChanged?.Invoke(currentMana);
        }
    }
}
