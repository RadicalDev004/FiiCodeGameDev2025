using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour
{
    public GameObject Canvas;
    public Slider S_Health;
    public float MaxHealth = 0, CurrentHealth, Speed = 1, Damage = 50, AttackSpeed = 1, AttackRange = 1, BaseAttackRange = 0.1f, BaseAttackSpeed = 1;
    public float givenManaPerHit, Dist;
    public bool Healing = false, Attacking = false;
    public int Difficulty = -1;
    public GameObject AttackRangeObj;
    public float AttackObjLocalScaleFactor = 4;
    private PlayerBehaviour PlayerBehaviour;
    public float lookToPlayerRotationSpeed = 0.5f;
    public float totalSpeed = 0.1f;
    public FunctionItem OrgFunctionItem;
    public SkinnedMeshRenderer Body;
    public List<Material> Stages = new();

    void Start()
    {
        PlayerBehaviour = Ref.PlayerBehaviour;        
    }

    private void Update()
    {
        Canvas.transform.LookAt(PlayerBehaviour.gameObject.transform.position);
        Dist = Vector3.Distance(transform.position, PlayerBehaviour.transform.position);
        if ( Dist <= AttackRange * BaseAttackRange && !Attacking)
        {
            Attacking = true;
            StartCoroutine(AttackCor(BaseAttackSpeed / AttackSpeed));
        }

        if (Attacking) return;

        Vector3 direction = PlayerBehaviour.transform.position - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookToPlayerRotationSpeed * Speed * Time.deltaTime);
        }
        transform.Translate(totalSpeed * Speed * Time.deltaTime * Vector3.forward);

        if(Healing && CurrentHealth < MaxHealth)
        {
            CurrentHealth += Time.deltaTime * 4f;
            S_Health.value = CurrentHealth;

            if (CurrentHealth > MaxHealth)
                CurrentHealth = MaxHealth;
        }
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

    public void Create(float MaxHealth, float Speed, float Damage, float AttackSpeed,float AttackRange, bool Healing, int Difficulty = 0, bool updateCode = true)
    {
        string code = $"\r\nenemy.speed = <e>{Speed}</e>;\r\n\r\n" +
            $"enemy.damage = <e>{Damage}</e>;\r\n\r\n" +
            $"enemy.attack_speed = <e>{AttackSpeed}</e>;\r\n\r\n" +
            $"enemy.attack_range = <e>{AttackRange}</e>;\r\n\r\n" +
            $"enemy.healing = <e>{Healing}</e>;\r\n\r\n" +
            /*"if(player_in_range())\r\n" +
            "{\r\n   " +
            $"attack_obj = <e>player</e>;\r\n   " +
            "damage(attack_obj);\r\n" +
            "}\r\n\r\n" +*/
            "<color=#031700>/* \r\n YOU CANNOT EDIT MORE THAN 1 VALUE AT ONCE \r\n 0.5 <= speed <= 5, 1 <= damage <= 100, 0.5 <= attack_speed <= 5, 0.5 <= attack_range <= 5, healing = true | false, */</color>\r\n\r\n";

        if(updateCode)
            GetComponentInChildren<Enemy>().ExecutableCode = code;

        if(MaxHealth != this.MaxHealth)
        {
            this.MaxHealth = MaxHealth;
            CurrentHealth = MaxHealth;
        }       

        S_Health.maxValue = MaxHealth;
        S_Health.value = CurrentHealth;

        this.Speed = Speed;
        this.Damage = Damage;
        this.Healing = Healing;
        this.AttackSpeed = AttackSpeed;
        this.Difficulty = Difficulty;
        this.AttackRange = AttackRange;

        Material[] newMaterials = Body.materials;
        newMaterials[0] = Stages[this.Difficulty];
        Body.materials = newMaterials;
    }


    public void Death()
    {
        for(int i = 0; i <= Difficulty; i++)
        {
            FunctionItem fi = Instantiate(OrgFunctionItem, OrgFunctionItem.transform.parent);
            fi.transform.SetParent(null);
            fi.gameObject.SetActive(true);
            fi.Create();
        }
        
        Destroy(gameObject);
    }

    private IEnumerator AttackCor(float duration)
    {
        AttackRangeObj.SetActive(true);
        AttackRangeObj.transform.localScale = new(AttackObjLocalScaleFactor * AttackRange, AttackRangeObj.transform.localScale.y, AttackObjLocalScaleFactor * AttackRange);

        AttackRangeObj.GetComponent<MeshRenderer>().material.color = new Color32(255, 0, 0, 0);
        StartCoroutine(ChangeAlpha(AttackRangeObj.GetComponent<MeshRenderer>().material, duration));
        yield return new WaitForSeconds(duration);

        if(Vector3.Distance(transform.position, PlayerBehaviour.transform.position) <= AttackRange * BaseAttackRange)
        {
            PlayerBehaviour.TakeDamage(Damage);
        }
        AttackRangeObj.SetActive(false);
        Attacking = false;
    }

    private IEnumerator ChangeAlpha(Material mat, float duration)
    {
        if (mat == null) yield break;

        Color color = mat.color;
        float startAlpha = color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 1f, elapsed / duration);
            mat.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        color.a = 1f;
        mat.color = color;
    }
}
