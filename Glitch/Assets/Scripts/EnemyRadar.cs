using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnemyRadar : MonoBehaviour
{
    public RectTransform OrgRadar;
    public Transform Cam;
    public Dictionary<EnemyBehaviour, RectTransform> AllRadars = new();
    public static bool isActive;

    public float MinDist = 0.1f, MaxDist = 1.5f;

    private void Update()
    {
        var keysToRemove = new List<EnemyBehaviour>();

        foreach (var pair in AllRadars)
        {
            if (pair.Key == null)
            {
                Destroy(pair.Value.gameObject);
                keysToRemove.Add(pair.Key);
                continue;
            }

            pair.Value.GetComponent<CanvasGroup>().alpha = isActive ? 1 : 0;

            Vector3 enemyPos = pair.Key.transform.position;
            Vector3 playerPos = Cam.position;

            Vector2 playerPos2D = new(playerPos.x, playerPos.z);
            Vector2 enemyPos2D = new(enemyPos.x, enemyPos.z);

            Vector2 toEnemy2D = (enemyPos2D - playerPos2D).normalized;

            Vector2 playerForward2D = new Vector2(Cam.forward.x, Cam.forward.z).normalized;

            float angle = Vector2.SignedAngle(playerForward2D, toEnemy2D);
            if (angle < 0) angle += 360f;

            pair.Value.rotation = Quaternion.Euler(0, 0, angle);

            Color c = pair.Value.transform.GetChild(0).GetComponent<Image>().color;
            c.a = DistanceToAlpha(Vector3.Distance(Ref.PlayerBehaviour.gameObject.transform.position, enemyPos)) / 255;
            pair.Value.transform.GetChild(0).GetComponent<Image>().color = c;
        }
        foreach (var key in keysToRemove)
        {
            AllRadars.Remove(key);
        }
    }

    public void AddRadar(EnemyBehaviour enemy)
    {
        RectTransform newRadar = Instantiate(OrgRadar, OrgRadar.transform.parent);
        newRadar.gameObject.SetActive(true);
        newRadar.SetAsFirstSibling();
        AllRadars.Add(enemy, newRadar);
    }

    public float DistanceToAlpha(float distance)
    {
        if(distance < MinDist) distance = MinDist;
        if(distance > MaxDist) distance = MaxDist;

        return Mathf.Lerp(255f, 25f, Mathf.InverseLerp(MinDist, MaxDist, distance));
    }
}
