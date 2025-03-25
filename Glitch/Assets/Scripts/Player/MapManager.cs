using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public GameObject mapUI; 
    public List<Button> checkpointButtons; 
    private Dictionary<Button, Vector3> teleportLocations = new Dictionary<Button, Vector3>();

    void Start()
    {
        mapUI.SetActive(false);

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
        //Cursor.visible = isActive;
        if (isActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
        }
        else
        {
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
