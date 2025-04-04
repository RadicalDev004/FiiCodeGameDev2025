using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using JetBrains.Annotations;

public class AdminCommands : MonoBehaviour
{
    [SerializeField] private TMP_InputField speedInput;
    [SerializeField] private TMP_InputField jumpInput;
    [SerializeField] private TMP_InputField checkpointInput;
    [SerializeField] private List<Checkpoint> checkpoints = new List<Checkpoint>();
    public void SolveCurrentPuzzle()
    {
        if (Ref.PlayerBehaviour.GetEditable() != null)
        {
            Editable editable = Ref.PlayerBehaviour.GetEditable();
            editable.CloseTerminal();
            editable.OnGlitchSolve();
        }
        else
            return; 
    }

    public void KillAllEnemies()
    {
        if (Ref.EnemySpawner.AllEnemies.Count > 0)
        {
            List<EnemyBehaviour> enemies = Ref.EnemySpawner.AllEnemies;

            foreach (EnemyBehaviour enemy in enemies)
            {
                enemy.Death();
            }
            Ref.EnemySpawner.AllEnemies.Clear();
        }
    }

    public void SetSpeedJump()
    {
        float speed = 0.2f, jump = 0.05f;
        if (!string.IsNullOrEmpty(speedInput.text))
        {
            if (float.TryParse(speedInput.text, out float sp))
            {
                speed *= sp;
            }
        }
        if (!string.IsNullOrEmpty(jumpInput.text))
        {
            if (float.TryParse(jumpInput.text, out float jmp))
            {
                jump *= jmp;
            }
        }

        Debug.Log("Speed: " + speed + " Jump: " + jump);
        Ref.Movement.jumpForce = jump ;
        Ref.Movement.speed = speed;
    }

    public void UnlockAll()
    {
        foreach(Checkpoint cp in checkpoints)
        {
            Ref.MapManager.UnlockCheckpoint(cp.checkpointButton, cp.teleportLocation); 
        }
    }
}
