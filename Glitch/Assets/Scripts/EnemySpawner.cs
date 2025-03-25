using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject OriginalEnemy;
    private PlayerBehaviour PlayerBehaviour;

    public float yOffsetUp, yOffsetDown;

    private void Awake()
    {
        PlayerBehaviour = Ref.PlayerBehaviour;
    }

    public void SpawnEnemies(float radius = 1, params int[] difficulty)
    {
        Debug.LogWarning("Spawn: " + string.Join(", ", difficulty));
        int ind = 0;
        while(ind < difficulty.Length)
        {
            float randomRadius = Mathf.Sqrt(Random.Range(0.1f, 1f)) * radius;
            float angle = Random.Range(0f, Mathf.PI * 2);
            Vector2 rnd = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * randomRadius;

            Vector3 newEnemyPos = PlayerBehaviour.gameObject.transform.position + new Vector3(rnd.x, yOffsetUp, rnd.y);

            Ray ray = new(newEnemyPos, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, yOffsetUp + yOffsetDown) && Mathf.Abs(hit.point.y - PlayerBehaviour.transform.position.y) < 0.1f)
            {
                Vector3 hitPoint = hit.point;
                SpawnEnemyBasedOnDifficulty(hitPoint + new Vector3(0, yOffsetDown, 0), difficulty[ind]);

                ind++;
            }
        }
        //Debug.Break();
    }

    public void SpawnEnemyBasedOnDifficulty(Vector3 position, int difficulty)
    {
        int MaxHealth;
        float Speed;
        float Damage;
        bool Healing;
        float AttackSpeed;

        switch(difficulty)
        {
            default:
            case 0:
                MaxHealth = 25 * Random.Range(2, 6);
                Speed = 0.25f * Random.Range(3, 6);
                Damage = 5 * Random.Range(3, 6);
                Healing = RandomChance.Percent(10);
                AttackSpeed = 1;
            break;

            case 1:
                MaxHealth = 25 * Random.Range(3, 8);
                Speed = 0.25f * Random.Range(4, 6);
                Damage = 5 * Random.Range(4, 7);
                Healing = RandomChance.Percent(20);
                AttackSpeed = 1 + 0.2f * Random.Range(0, 3);
            break;

            case 2:
                MaxHealth = 25 * Random.Range(4, 9);
                Speed = 0.25f * Random.Range(5, 7);
                Damage = 5 * Random.Range(5, 9);
                Healing = RandomChance.Percent(30);
                AttackSpeed = 1 + 0.2f * Random.Range(1, 5);
            break;

            case 3:
                MaxHealth = 25 * Random.Range(5, 10);
                Speed = 0.25f * Random.Range(6, 8);
                Damage = 5 * Random.Range(6, 10);
                Healing = RandomChance.Percent(50);
                AttackSpeed = 1 + 0.2f * Random.Range(3, 7);
            break;

            case 4:
                MaxHealth = 25 * Random.Range(10, 15);
                Speed = 0.25f * Random.Range(8, 12);
                Damage = 5 * Random.Range(10, 15);
                Healing = RandomChance.Percent(90);
                AttackSpeed = 1 + 0.2f * Random.Range(4, 9);
            break;
        }

        SpawnEnemy(position, MaxHealth, Speed, Damage, Healing, AttackSpeed, difficulty);
    }

    public void SpawnEnemy(Vector3 position, int MaxHealth, float Speed, float Damage, bool Healing, float AttackSpeed, int Difficulty)
    {
        Debug.LogError("new enemy pos: " + position);
        GameObject newEnemy = Instantiate(OriginalEnemy, position, Quaternion.identity, OriginalEnemy.transform.parent);
        newEnemy.SetActive(true);
        newEnemy.GetComponent<EnemyBehaviour>().Create(MaxHealth, Speed, Damage, AttackSpeed, Healing, Difficulty);
        newEnemy.tag = "Enemy";
    }
}
