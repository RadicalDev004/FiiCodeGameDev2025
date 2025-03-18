using System;
using UnityEngine;

public class ManaSystem : MonoBehaviour
{
    public static ManaSystem Instance;
    public event Action<float> OnManaChanged;

    private float currentMana = 0;
    private float maxMana = 1;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        currentMana = maxMana;
    }

    public void AddMana(float amount)
    {
        if(currentMana < maxMana)
        {
            currentMana += amount;
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
