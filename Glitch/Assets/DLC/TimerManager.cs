using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PuzzleTimerDisplay
{
    public string puzzleName;
    public float time;
}

[System.Serializable]
public class PuzzleTimerConfig
{
    [TextArea(10, 10)]
    public string instructions;
    public string puzzleName;
    public int questionsAvailable = 2;
    public float timeThreshold = 60f;
    public bool hintTriggered = false;
}


public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; }

    [Header("Puzzle Timer Configs")]
    public List<PuzzleTimerConfig> puzzleConfigs = new();

    private Dictionary<string, float> timers = new();
    private HashSet<string> activeTimers = new();

    [Header("Debug View (Read-Only)")]
    [SerializeField] private List<PuzzleTimerDisplay> debugTimers = new();


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        foreach (string puzzle in activeTimers)
        {
            if (!timers.ContainsKey(puzzle))
                timers[puzzle] = 0f;

            timers[puzzle] += Time.unscaledDeltaTime;

            CheckThreshold(puzzle, timers[puzzle]);
        }

        UpdateDebugTimers();
    }

    public void StartTimer(string puzzleName)
    {
        activeTimers.Add(puzzleName);
        Debug.Log($"Started timing for {puzzleName}");
    }

    public void AskAi()
    {
        string ask = Ref.Tutorial.inputQuestion.text;
        
    }

    public void StopTimer(string puzzleName)
    {
        activeTimers.Remove(puzzleName);
        Debug.Log($"Stopped timing for {puzzleName}, total: {GetTime(puzzleName):F2}s");
    }

    public float GetTime(string puzzleName)
    {
        return timers.TryGetValue(puzzleName, out float t) ? t : 0f;
    }

    private void CheckThreshold(string puzzleName, float time)
    {
        PuzzleTimerConfig config = puzzleConfigs.Find(p => p.puzzleName == puzzleName);
        if (config != null && !config.hintTriggered && time >= config.timeThreshold)
        {
            config.hintTriggered = true;

            Debug.Log($"Hint triggered for {puzzleName}");
            if (Ref.Tutorial != null && Ref.Tutorial.Enabled)
            {
                Ref.Tutorial.GetHintAI(config.instructions, config);
            }
        }
    }

    private void UpdateDebugTimers()
    {
        debugTimers.Clear();
        foreach (var kvp in timers)
        {
            debugTimers.Add(new PuzzleTimerDisplay
            {
                puzzleName = kvp.Key,
                time = kvp.Value
            });
        }
    }
}