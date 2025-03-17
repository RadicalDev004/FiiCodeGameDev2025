using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour
{
    public GameObject Canvas;
    public Slider S_Health;
    public float MaxHealth = 100, CurrentHealth, Speed = 100, Damage = 50;
    public float givenManaPerHit;
    public bool Healing = false;
    private PlayerBehaviour PlayerBehaviour;

    void Start()
    {
        CurrentHealth = MaxHealth;
        PlayerBehaviour = Ref.PlayerBehaviour;

        S_Health.maxValue = MaxHealth;
        S_Health.value = CurrentHealth;
    }

    private void Update()
    {
        Canvas.transform.LookAt(PlayerBehaviour.gameObject.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Projectile proj))
        {
            UpdateHealthSlider(-proj.damage);
            ManaSystem.Instance.AddMana(givenManaPerHit);
        }
    }

    public void UpdateHealthSlider(float value)
    {
        CurrentHealth += value;

        if (CurrentHealth <= 0)
        {
            Death();
            return;
        }

        S_Health.value = CurrentHealth;
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}
