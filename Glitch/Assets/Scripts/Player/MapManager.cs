using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Pixelplacement;
using TMPro;
using System.Collections;

public class MapManager : MonoBehaviour
{
    public GameObject mapUI; 
    public Dictionary<Button, Vector3> teleportLocations = new();
    public static bool IsOpen = false;

    public void ToggleMap(bool state)
    {
        if (state && (Settings.IsOpen || Code.IsOpen)) return;

        IsOpen = state;

       if (state) mapUI.SetActive(true);

        mapUI.transform.GetChild(0).localScale = Vector3.one * (state ? 0 : 1);
        Tween.LocalScale(mapUI.transform.GetChild(0), Vector3.one * (state ? 1 : 0), 0.2f, 0, Tween.EaseInOut);

        if (IsOpen)
        {
            LookPC.isPaused = state;
            Movement.IsPaused = state;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
        }
        else
        {
            StartCoroutine(ResetMapAfterCloseCor(0.2f));
        }
       
    }

    private IEnumerator ResetMapAfterCloseCor(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        mapUI.SetActive(false);
        LookPC.isPaused = false;
        Movement.IsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
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

    public void TeleportPlayer(Button button)
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
        if (Ref.EnemySpawner.AllEnemies.Count > 0)
        {
            Debug.Log(Ref.EnemySpawner.AllEnemies.Count);
            return true;
        }
        else    
            return false;

    }
}
