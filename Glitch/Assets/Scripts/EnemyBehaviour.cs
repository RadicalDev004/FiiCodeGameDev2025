using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class EnemyBehaviour : MonoBehaviour
{
    public GameObject Canvas;
    public Slider S_Health;
    public float MaxHealth = 100, CurrentHealth, Speed = 100, Damage = 50;
    public float givenManaPerHit;
    public bool Healing = false;
    private PlayerBehaviour PlayerBehaviour;
    public float lookToPlayerRotationSpeed = 0.5f;
    public float moveSpeed = 1;

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

        Vector3 direction = PlayerBehaviour.transform.position - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookToPlayerRotationSpeed * Time.deltaTime);
        }
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Projectile proj))
        {
            UpdateHealthSlider(-proj.damage);
            ManaSystem.Instance.AddMana(givenManaPerHit);
            Destroy(proj.gameObject);
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
