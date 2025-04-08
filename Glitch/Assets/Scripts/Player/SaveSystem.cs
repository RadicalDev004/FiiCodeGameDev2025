using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public Vector3 LastPos;

    public float Health;

    public Dictionary<string, int> Functions = new();

    public static Editable LatestSolvedGlitch;

    public float Mana;

    private void Start()
    {
        SaveState();
    }

    public void SaveState()
    {
        Functions = Ref.Inventory.Functions;

        Mana = Ref.ManaSystem.currentMana;

        LastPos = Ref.PlayerBehaviour.transform.position;
        Health = Ref.PlayerBehaviour.CurrentHealth;        
    }

    public void LoadState()
    {
        Debug.LogWarning("LOADING PREVIOUS STATE");

        if (LatestSolvedGlitch != null)
            LatestSolvedGlitch.OnGlitchSolve(true);

        Ref.Inventory.Functions = Functions;
        Ref.Inventory.LegendaryFunctions = new();

        Ref.ManaSystem.currentMana = Mana;
        Ref.ManaUI.UpdateManaUI(Mana);

        Ref.PlayerBehaviour.GetComponent<CharacterController>().enabled = false;
        Ref.PlayerBehaviour.transform.position = LastPos;
        Ref.PlayerBehaviour.GetComponent<CharacterController>().enabled = true;

        PlayerBehaviour.ProjectileSizeIncrease = 0;
        ManaSystem.ExtraManaPerHit = 0;

        Ref.PlayerBehaviour.ResetHealth(Health);

        FindObjectOfType<BossBubble>().ResetState();

        Ref.EnemySpawner.RemoveAllEnemies();

        FunctionItem.ClearAllItems();
    }
}
