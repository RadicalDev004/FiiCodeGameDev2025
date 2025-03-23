using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FunctionItem : MonoBehaviour
{
    public static List<string> functionNames = new() { "heal()", "attack_boost()", "speed_increase()" };
    public string Name;
    private TMP_Text T_Name;
    private Rigidbody Rigidbody;


    public void Create()
    {
        string fnc = functionNames[Random.Range(0, functionNames.Count)];

        T_Name = GetComponentInChildren<TMP_Text>();
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.AddForce(new Vector3(Random.Range(0.3f,0.75f), Random.Range(0.5f, 1.5f), Random.Range(0.3f, 0.75f)), ForceMode.Impulse);

        Name = fnc.ToLower();
        T_Name.text = fnc + ";";        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Inventory inventory))
        {
            inventory.CollectFunction(Name);
            Destroy(gameObject);
        }
    }
}
