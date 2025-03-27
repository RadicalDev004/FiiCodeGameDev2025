using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Checkpoint : MonoBehaviour
{
    public Button checkpointButton;
    public Vector3 teleportLocation;
    public UI _UI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MapManager mapController = FindObjectOfType<MapManager>();
            if (mapController != null)
            {
                mapController.UnlockCheckpoint(checkpointButton, teleportLocation);
            }

            if (Ref.UI != null && gameObject.name != "Tutorial")
            {
                Ref.UI.ToggleCheckpointText();
            }

            Destroy(gameObject);
        }
    }
}
