using UnityEngine;
using UnityEngine.UI;

public class Checkpoint : MonoBehaviour
{
    public Button checkpointButton;
    public Vector3 teleportLocation;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MapManager mapController = FindObjectOfType<MapManager>();
            if (mapController != null)
            {
                mapController.UnlockCheckpoint(checkpointButton, teleportLocation);
            }
            Destroy(gameObject);
        }
    }
}
