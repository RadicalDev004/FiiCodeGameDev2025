using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalBoss : MonoBehaviour
{
    public Slider S_Shield, S_Health;
    public float MaxHealth = 500, MaxShield = 60;

    public float ShieldHealth { get { return S_Shield.value; } set { S_Shield.value = value; } }
    public float Health { get { return S_Health.value; } set { S_Health.value = value; } }

    public bool isShielded;
    public GameObject Shield;
    public BossCrystal OrgCrystal;
    
    public List<BossCrystal> CrystalList;
    public int CurrentCrytals;
    public BossProj Projectile;

    public int Phase = 1;
    public int TotalPhases = 5;

    public Coroutine ShootProjCor;


    void OnEnable()
    {
        Shield.SetActive(true);
        S_Shield.maxValue = MaxShield;
        ShieldHealth = 0;

        S_Health.maxValue = MaxHealth;
        Health = MaxHealth;

        Ref.ActionAfterTime(3, delegate
        {
            ReShield(5);

            Ref.ActionAfterTime(5, delegate {
                SpawnCrystals(Phase);
                });
        });
    }

    

    public void SpawnCrystals(int cnt)
    {
        CurrentCrytals = cnt + 1;
        for(int i = 0; i <= cnt; i++)
        {
            float spawnX = Random.Range(0, 1f);
            float spawnZ = Random.Range(0, 1f);

            BossCrystal cr = Instantiate(OrgCrystal);
            cr.transform.position = new Vector3(Ref.PlayerBehaviour.transform.position.x + spawnX, cr.transform.position.y, Ref.PlayerBehaviour.transform.position.z + spawnZ);
            cr.gameObject.SetActive(true);
            cr.Create(i);
            cr.FinalBoss = this;
            CrystalList.Add(cr);
        }
        
    }
    

    public void ReShield(float time)
    {
        if(Phase < 5) StartCoroutine(WaitForNextPhase());
        Shield.SetActive(true);
        isShielded = true;
        Tween.Value(ShieldHealth, MaxShield, health => ShieldHealth = health, time, 0, Tween.EaseOut);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Projectile proj))
        {
            TakeDamage(proj.damage);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isShielded) return;
        Health -= damage;
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void DeShield()
    {
        Shield.SetActive(false);
        isShielded = false;

        StartPhase();
    }

    public void DefeatedCrystal(BossCrystal bc)
    {
        CrystalList.Remove(bc);

        Tween.Value(ShieldHealth, MaxShield / CurrentCrytals * CrystalList.Count, val => ShieldHealth = val ,3, 0, Tween.EaseOut, obeyTimescale: true);

        if(CrystalList.Count == 0)
        {
            DeShield();
        }
    }

    private IEnumerator WaitForNextPhase()
    {
        yield return new WaitUntil(() => Health < MaxHealth / TotalPhases * (TotalPhases - Phase));

        StopCoroutine(ShootProjCor);
        

        Phase++;
        ReShield(5);

        Ref.ActionAfterTime(5, delegate {
            SpawnCrystals(Phase);
        });
    }

    private IEnumerator ShootProjectilesCor(float time)
    {
        while(true)
        {
            yield return new WaitForSeconds(time);

            BossProj prj = Instantiate(Projectile, Projectile.transform.parent);
            prj.transform.SetParent(null);
            prj.gameObject.SetActive(true);
            prj.transform.LookAt(Ref.PlayerBehaviour.transform);
            prj.Shoot(5, 1 + 0.2f * (Phase - 1), 1, 15);

        }
    }

    public void StartPhase()
    {
        ShootProjCor = StartCoroutine(ShootProjectilesCor(1));
        if (Phase < 2) return;
    }

    public void ResetState()
    {
        foreach(var cr in CrystalList)
        {
            Destroy(cr);
        }
        CrystalList.Clear();

        Phase = 1;
    }
}
