using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PuzzleZone : MonoBehaviour
{
    public string puzzleName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TimerManager.Instance.StartTimer(puzzleName);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TimerManager.Instance.StopTimer(puzzleName);
        }
    }
}
