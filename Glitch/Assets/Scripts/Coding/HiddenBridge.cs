using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenBridge : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private BoxCollider boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        meshRenderer = transform.parent.GetChild(1).GetComponent<MeshRenderer>();
        meshCollider = transform.parent.GetChild(1).GetComponent<MeshCollider>();

        meshRenderer.enabled = false;
        meshCollider.enabled = false;
        boxCollider.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            Debug.Log("Am detectat un proiectil");
            meshRenderer.enabled = true;
            meshCollider.enabled = true;
            boxCollider.isTrigger = false;

            //Destroy(other.gameObject); // optional daca vrei  ca proiectilu sa nu treaca prin el da na merge
        }
    }
}
