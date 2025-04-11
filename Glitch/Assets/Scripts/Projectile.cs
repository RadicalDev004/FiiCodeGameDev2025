using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed, damage = 10;
    public GameObject impactParticles;
    public static bool PassThrough = false;

    private void Update()
    {
        transform.Translate((BossCrystal.C3 ? speed / 2 : speed) * Time.deltaTime * Vector3.forward);
    }

    public void Shoot(float lifetime, float speed, float size, float damage)
    {
        this.speed = speed;
        this.damage = damage;
        transform.localScale = transform.localScale * size;
        StartCoroutine(Lifetime(lifetime));
    }

    private IEnumerator Lifetime(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("checkpoint"))
        {
            AudioManager.Play("Projectile_Hit");
            Instantiate(impactParticles, transform.position, Quaternion.identity);
            //Debug.Break();
            if(!PassThrough)
                Destroy(gameObject);
        }
    }

}
