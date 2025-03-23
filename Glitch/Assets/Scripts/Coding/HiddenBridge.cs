using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenBridge : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        boxCollider.enabled = true;
        boxCollider.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            Debug.Log("Am detectat un proiectil");
            meshRenderer.enabled = true;
            boxCollider.isTrigger = false;
            Destroy(other.gameObject); // optional daca vrei  ca proiectilu sa nu treaca prin el da na merge
        }
    }
}
