using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FunctionItem : MonoBehaviour
{
    public static List<string> functionNames = new() { "heal()", "attack_boost()", "speed_increase()", "attack_speed()", "projectile_through()" };
    public static List<string> legendaryFunctions = new() { "increase_max_health()", "increase_projectile_size()", "more_mana_per_hit()" };
    public string Name;
    private TMP_Text T_Name;
    private Rigidbody Rigidbody;
    public bool isLegendary;


    public void Create(bool isLegendary = false)
    {
        this.isLegendary = isLegendary;
        string fnc = isLegendary ? legendaryFunctions[Random.Range(0, legendaryFunctions.Count)] : functionNames[Random.Range(0, functionNames.Count)];

        T_Name = GetComponentInChildren<TMP_Text>();
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.AddForce(new Vector3(Random.Range(0.3f,0.75f), Random.Range(0.5f, 1.5f), Random.Range(0.3f, 0.75f)), ForceMode.Impulse);

        Name = fnc.ToLower();
        T_Name.text = $"{(isLegendary ? "<color=orange>" : "")}{fnc}{(isLegendary ? "</color>" : "")};";        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Inventory inventory))
        {
            if (isLegendary)
                inventory.CollectLegendaryFunction(Name);
            else
                inventory.CollectFunction(Name);
            Destroy(gameObject);
        }
    }
}
