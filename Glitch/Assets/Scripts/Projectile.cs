using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed, damage = 10;

    private void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.forward);
    }

    public void Shoot(float lifetime, float speed, float size)
    {
        this.speed = speed;
        transform.localScale = transform.localScale * size;
        StartCoroutine(Lifetime(lifetime));
    }

    private IEnumerator Lifetime(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
