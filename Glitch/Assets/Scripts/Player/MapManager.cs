using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public GameObject mapUI; // UI-ul pentru hartă
    public List<Button> checkpointButtons; // Lista de butoane pentru checkpoint-uri
    private Dictionary<Button, Vector3> teleportLocations = new Dictionary<Button, Vector3>();

    void Start()
    {
        // Inițial ascunde harta
        mapUI.SetActive(false);

        // Ascunde toate butoanele inițial
        foreach (Button button in checkpointButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMap();
        }
    }

    void ToggleMap()
    {
        bool isActive = !mapUI.activeSelf;
        mapUI.SetActive(isActive);
        Cursor.visible = isActive;
        Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked; // Blochează cursorul când harta e închisă
        Time.timeScale = isActive ? 0f : 1f; // Pune jocul pe pauză când harta e deschisă
    }

    public void UnlockCheckpoint(Button button, Vector3 worldPosition)
    {
        Debug.Log(button.name + " s-a activat");
        if (!teleportLocations.ContainsKey(button))
        {
            teleportLocations[button] = worldPosition;
            button.onClick.AddListener(() => TeleportPlayer(button));
            button.gameObject.SetActive(true); // Activează butonul pe hartă
        }
    }

    void TeleportPlayer(Button button)
    {
        Vector3 targetPosition = teleportLocations[button];
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false; 
            player.transform.position = targetPosition;
            controller.enabled = true; 
        }

        player.transform.position = targetPosition;
        ToggleMap();
    }
}
