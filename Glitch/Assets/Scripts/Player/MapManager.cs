using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public GameObject mapUI; 
    private Dictionary<Button, Vector3> teleportLocations = new();
    public static bool IsOpen = false;


    public void ToggleMap(bool state)
    {
        IsOpen = state;
        mapUI.SetActive(IsOpen);
        //Cursor.visible = isActive;
        if (IsOpen)
        {
            LookPC.isPaused = state;
            Movement.IsPaused = state;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
        }
        else
        {
            LookPC.isPaused = state;
            Movement.IsPaused = state;
            Cursor.lockState= CursorLockMode.Locked;
            Time.timeScale = 1f;
        }
       
    }

    public void UnlockCheckpoint(Button button, Vector3 worldPosition)
    {
        Debug.Log(button.name + " s-a activat");
        if (!teleportLocations.ContainsKey(button))
        {
            teleportLocations[button] = worldPosition;
            button.onClick.AddListener(() => TeleportPlayer(button));
            button.gameObject.SetActive(true);
        }
    }

    void TeleportPlayer(Button button)
    {
        if(!AreZombiesAlive())
        {
            Vector3 targetPosition = teleportLocations[button];
            GameObject player = Ref.PlayerBehaviour.gameObject;
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                player.transform.position = targetPosition;
                controller.enabled = true;
            }

            ToggleMap(false);
        }
        else
        {
            Ref.UI.ToggleZombiesAlive();
        }
    }

    public bool AreZombiesAlive()
    {
        //GameObject[] zombies = GameObject.FindGameObjectsWithTag("Enemy");
        if (EnemySpawner.AllEnemies.Count > 0)
            return true;
        else
            return false;
    }
}
