using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAiLevel : MonoBehaviour
{
    public TileManager TileManager;

    private void OnTriggerEnter(Collider other)
    {
        TileManager.ReloadCurrentLevel();
    }
}
