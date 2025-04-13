using Pixelplacement.TweenSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

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
    public TMP_Text T_Think;
    public float InitialTime = 3, InBetweenTime = 3;
    public int StepToGiveItems;
    public List<GameObject> ObjectsToGive = new();
    public GameObject GhostParent;
    public BoolSlider Toggle;

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
        LookPC.isPaused = false;
        GhostAnimator.SetTrigger("endTutorial");

        yield return new WaitForSeconds(2);
        GhostAnimator.gameObject.SetActive(false);
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
