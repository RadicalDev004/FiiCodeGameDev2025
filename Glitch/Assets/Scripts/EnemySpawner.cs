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

    public void SpawnEnemies(int amount, int radius = 1)
    {
        while(amount > 0)
        {
            Vector2 rnd = Random.insideUnitCircle * radius;
            Vector3 newEnemyPos = PlayerBehaviour.gameObject.transform.position + new Vector3(rnd.x, yOffsetUp, rnd.y);

            Ray ray = new(newEnemyPos, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, yOffsetUp + yOffsetDown))
            {
                Vector3 hitPoint = hit.point;

                GameObject newEnemy = Instantiate(OriginalEnemy, hitPoint + new Vector3(0, yOffsetDown, 0), Quaternion.identity, OriginalEnemy.transform.parent); 
                newEnemy.SetActive(true);

                amount--;
            }
        }
        
    }
}
