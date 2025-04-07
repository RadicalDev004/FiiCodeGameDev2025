using Pixelplacement;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Checkpoint : MonoBehaviour
{
    public Button checkpointButton;
    public GameObject checkpointObj;
    public Vector3 teleportLocation;

    private void Start()
    {
        if (checkpointObj != null)
        {
            Vector3 finalPos = checkpointObj.transform.localPosition + new Vector3(0, -1f, 0);

            checkpointObj.transform.localPosition = finalPos;
            checkpointObj.transform.localScale = Vector3.one;
            checkpointObj.transform.GetChild(0).GetComponent<Light>().enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(hoverCheckpoint());

            MapManager mapController = FindObjectOfType<MapManager>();
            if (mapController != null)
            {
                mapController.UnlockCheckpoint(checkpointButton, teleportLocation);
            }

            if (Ref.UI != null && gameObject.name != "Tutorial")
            {
                AudioManager.Play("Checkpoint_Unlocked");
                Ref.UI.ToggleCheckpointText();
            }

            Destroy(gameObject);
        }
    }

    private IEnumerator hoverCheckpoint()
    {
        Vector3 finalPos = checkpointObj.transform.localPosition + new Vector3(0, 1f, 0);
        checkpointObj.transform.GetChild(0).GetComponent<Light>().enabled = true;

        Tween.LocalPosition(checkpointObj.transform, finalPos, 3, 0, Tween.EaseInOut);
        Tween.LocalScale(checkpointObj.transform, new Vector3(20f, 20f, 20f), 3, 0, Tween.EaseInOut);

        yield return new WaitForSeconds(2f);
    }
}
