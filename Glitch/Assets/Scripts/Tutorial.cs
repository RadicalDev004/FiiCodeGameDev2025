using Pixelplacement.TweenSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public bool Enabled = true, Ongoing = false;
    public float RotationCorrectionSpeed = 0.1f;
    public Animator GhostAnimator;
    public GameObject LookAt;
    public GameObject Camera;
    [TextArea(5, 5)]
    public List<string> Replies = new();
    [TextArea(5, 5)]
    public List<string> Repl1 = new();
    [TextArea(5, 5)]
    public List<string> Repl2 = new();
    [TextArea(5, 5)]
    public List<string> ReplyCustom = new();
    public TMP_Text T_Think;
    public float InitialTime = 3, InBetweenTime = 3;
    public int StepToGiveItems;
    public List<GameObject> ObjectsToGive = new();
    public GameObject GhostParent;
    public BoolSlider Toggle;
    public InputField inputQuestion;
    public PuzzleTimerConfig currentPuzzle;
    public GameObject objectsForInput;
    private string[] ultimeleReplici;

    void Start()
    {
        GhostAnimator.gameObject.SetActive(false);
        if (Enabled)
        {
            float en = PlayerPrefs.GetInt("Tutorial");
            Debug.LogWarning("Tutorial" + en);
            Enabled = en == 1;
        }
        if(!Enabled) 
        {
            foreach (var it in ObjectsToGive)
            {
                it.SetActive(true);
            }
            PlayerBehaviour.EnabledProjectiles = true;
        }
        else
        {
            StartCoroutine(ShowTutorial(InitialTime, InBetweenTime));
        }
        GhostParent.transform.localPosition = Enabled ? Vector3.zero : Vector3.one * 999;
        //if (!Enabled) return;
    }

    private void LateUpdate()
    {
        if(Ongoing && Enabled)
        {
            LookPC.isPaused = true;
            Vector3 direction = LookAt.transform.position - Camera.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            Camera.transform.rotation = Quaternion.Slerp(Camera.transform.rotation, targetRotation, RotationCorrectionSpeed * Time.deltaTime);
        }
    }

    private IEnumerator ShowTutorial(float initial, float time)
    {
        //prepare
        GhostAnimator.gameObject.SetActive(true);
        foreach (var it in ObjectsToGive)
        {
            it.SetActive(false);

        }
        PlayerBehaviour.EnabledProjectiles = false;


        //start tutorial
        yield return new WaitForSeconds(initial);
        Ongoing = true;
        
        int i = 0;
        foreach(var item in Replies)
        {
            if(i == StepToGiveItems)
            {
                foreach(var it in ObjectsToGive)
                {
                    it.SetActive(true);
                }
                PlayerBehaviour.EnabledProjectiles = true;
            }
            T_Think.text = item;
            yield return new WaitForSeconds(time);
            i++;
        }

        Ongoing = false;
        LookPC.isPaused = false;
        GhostAnimator.SetTrigger("endTutorial");

        yield return new WaitForSeconds(2);
        GhostAnimator.gameObject.SetActive(false);
        Ref.UI.TogglePressF(false);

    }

    public void ShowGenericInfo(List<string> info, float initial, float time)
    {
        if (!Enabled) return;

        float en = PlayerPrefs.GetInt("Tutorial");
        Enabled = en == 1;

        if (!Enabled) return;
        StartCoroutine(ShowGenericInfoCor(info, initial, time));
    }
    private IEnumerator ShowGenericInfoCor(List<string> info, float initial, float time)
    {
        Ref.UI.TogglePressF(true);
        GhostAnimator.SetTrigger("reset");
        GhostAnimator.gameObject.SetActive(true);

        yield return new WaitForSeconds(initial);
        Ongoing = true;

        foreach (var item in info)
        {
            T_Think.text = item;
            yield return new WaitForSeconds(time);
        }

        Ongoing = false;
        Ref.UI.TogglePressF(false);
        LookPC.isPaused = false;
        GhostAnimator.SetTrigger("endTutorial");
        yield return new WaitForSeconds(2);
        GhostAnimator.gameObject.SetActive(false);
    }

    public void ShowGenericHints(List<string> info, float initial, float time)
    {
        if (!Enabled) return;

        float en = PlayerPrefs.GetInt("Tutorial");
        Enabled = en == 1;

        if (!Enabled) return;
        StartCoroutine(ShowGenericHintsCor(info, initial, time));
    }
    private IEnumerator ShowGenericHintsCor(List<string> info, float initial, float time)
    {
        Movement.isPaused = true;
        PlayerBehaviour.EnabledProjectiles = false;
        Ref.UI.TogglePressF(true);
        GhostAnimator.SetTrigger("reset");
        GhostAnimator.gameObject.SetActive(true);

        yield return new WaitForSeconds(initial);
        Ongoing = true;

        foreach (var item in info)
        {
            T_Think.text = item;
            yield return new WaitForSeconds(time);
        }

        T_Think.text = "Have you got any questions?";
        objectsForInput.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Questions left: " + currentPuzzle.questionsAvailable + "/2";


        yield return new WaitForSeconds(0.5f);

        Cursor.lockState = CursorLockMode.None;
        objectsForInput.SetActive(true);
    }

    public void StopCurrentTutorial()
    {
        Movement.isPaused = false;
        PlayerBehaviour.EnabledProjectiles = true;
        Ongoing = false;
        LookPC.isPaused = false;
        GhostAnimator.SetTrigger("endTutorial");

        GhostAnimator.gameObject.SetActive(false);
        if (objectsForInput.activeInHierarchy)
            objectsForInput.SetActive(false);
        if (Cursor.lockState == CursorLockMode.None)
            Cursor.lockState = CursorLockMode.Locked;
        StopAllCoroutines();
    }

    public IEnumerator DelayStop()
    {
        yield return new WaitForSeconds(3f);
        StopCurrentTutorial();
    }

    private void ProcesezRaspuns(string raspuns)
    {
        ReplyCustom.Clear();

        string[] replici = raspuns.Split('#');
        ultimeleReplici = replici;
        foreach (string replica in replici)
        {
            string trimmed = replica.Trim();
            if (!string.IsNullOrEmpty(trimmed))
                ReplyCustom.Add(trimmed);
        }

        ShowGenericHints(ReplyCustom, InitialTime, InBetweenTime);

        Debug.Log("Replici AI primite: " + string.Join(" | ", ReplyCustom));    
    }


    public void GetHintAI(string instructions, PuzzleTimerConfig last)
    {
        currentPuzzle = last;
        if(Code.IsOpen)
            Ref.Code.CloseTerminal();
        //ShowGenericHints(new() {"dan"}, InitialTime, InBetweenTime);
        OpenAIChat.RequestChat(instructions, ProcesezRaspuns);
    }

    public void AskQuestionAI()
    {
        if(currentPuzzle.questionsAvailable > 0)
        {
            string questionFinal =
                "You are a mysterious AI assistant in a Unity puzzle game. The player is currently solving a puzzle. Here is the full puzzle context:\n\n" +
                "---\n" +
                currentPuzzle.instructions +
                "---\n\n" +
                "The player previously received cryptic hints from you about this puzzle: " +
                "---\n"+
                 string.Join(", ", ultimeleReplici) + "\n" +
                ". Now they have a follow-up question.\n\n" +
                "Answer the question with clarity, but:\n" +
                "- Do not reveal the full solution.\n" +
                "- Only talk about the current puzzle.\n" +
                "- Be thematic and subtle when needed.\n" +
                "- Stay in-character as a mysterious assistant.\n\n" +
                $"Player’s question: \"{objectsForInput.transform.GetChild(0).GetComponent<TMP_InputField>().text}\"\n\n" +
                "Answer:";
            currentPuzzle.questionsAvailable -= 1;
            objectsForInput.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Questions left: " + currentPuzzle.questionsAvailable + "/2";
            //Debug.Log("Intrebare finala: " + questionFinal);
            objectsForInput.transform.GetChild(0).GetComponent<TMP_InputField>().text = "";
            T_Think.text = "Thinking...";
            OpenAIChat.RequestChat(questionFinal, ProcesezRaspuns);
        }
        else
        {
            T_Think.text = "No more questions available!";
            DelayStop();
        }
    }


    private void OnEnable()
    {
        Toggle.OnPress += OnToggle;
    }
    private void OnDisable()
    {
        Toggle.OnPress -= OnToggle;
    }
    public void OnToggle(bool state)
    {
        Enabled = state;
        if (!state)
        {
            foreach (var it in ObjectsToGive)
            {
                it.SetActive(true);
            }
            PlayerBehaviour.EnabledProjectiles = true;
        }
        GhostParent.transform.localPosition = state ? Vector3.zero : Vector3.one * 999;
    }
}
