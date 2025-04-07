using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossCrystal : MonoBehaviour
{
    public FinalBoss FinalBoss;

    public int Difficulty;
    public Slider S_Health;

    public List<Material> Materials;
    public float Health { get { return S_Health.value; } set { S_Health.value = value; } }

    public TMP_Text T_NegativeEffect;
    public GameObject Canvas;

    public static bool C2, C3, C4, C5;
    public List<Sprite> CustomIcons = new();
    public GameObject Laser;

    public FunctionItem OrgFunctionItem;

    private void Update()
    {
        Canvas.transform.LookAt(Ref.PlayerBehaviour.transform);

        if(Difficulty == 3)
        {
            Vector3 start = transform.position;
            Vector3 end = Ref.PlayerBehaviour.transform.position;

            Vector3 direction = end - start;
            float distance = direction.magnitude;

            Laser.transform.position = start + direction / 2;

            Laser.transform.up = direction;

            Laser.transform.localScale = new Vector3(Laser.transform.localScale.x, distance / 4, Laser.transform.localScale.z);

            Ref.PlayerBehaviour.TakeDamage(0.5f * Time.deltaTime);
        }
    }

    public void Create(int diff)
    {
        Difficulty = diff;
        GetComponent<MeshRenderer>().material = Materials[diff];
        
        S_Health.maxValue = (2 + 4 - diff) * Ref.PlayerBehaviour.Damage;
        Health = S_Health.maxValue;

        string clr, eff;
        switch(Difficulty)
        {
            case 0: clr = "green"; eff = ""; break;
            case 1: clr = "#009DFF"; eff = "slow_down(player);"; C2 = true; Ref.Inventory.AddCustomIcon("c2", CustomIcons[0]); break;
            case 2: clr = "#400036"; eff = "slow_projectile(player);"; C3 = true; Ref.Inventory.AddCustomIcon("c3", CustomIcons[1]); break;
            case 3: clr = "red"; eff = "slow_damage(player);"; C4 = true; Ref.Inventory.AddCustomIcon("c4", CustomIcons[2]); Laser.SetActive(true); break;
            case 4: clr = "orange"; eff = "no_mana(player);"; C5 = true; break;
            default: clr = ""; eff = ""; break;
        }
        T_NegativeEffect.text = $"<color={clr}>{eff}</color>";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Projectile proj))
        {
            TakeDamage(proj.damage);
            Ref.EnemySpawner.SpawnEnemies(1, Difficulty);
            Destroy(proj.gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        if(Health <= 0)
        {
            FinalBoss.DefeatedCrystal(this);
            switch (Difficulty)
            {
                case 0:break;
                case 1: Ref.Inventory.RemoveCustomIcon("c2"); C2 = false; break;
                case 2: Ref.Inventory.RemoveCustomIcon("c3"); C3 = false; break;
                case 3: Ref.Inventory.RemoveCustomIcon("c4"); C4 = false; break;
                case 4: C5 = false; break;
                default:break;
            }

            if(RandomChance.Percent(75))
            {
                
                FunctionItem fi = Instantiate(OrgFunctionItem, OrgFunctionItem.transform.parent);
                fi.transform.SetParent(null);
                fi.gameObject.SetActive(true);
                fi.Create(true);
            }
            

            Destroy(gameObject);
        }
    }
}
