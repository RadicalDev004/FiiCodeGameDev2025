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
    public TMP_Text T_Think;
    public float InitialTime = 3, InBetweenTime = 3;
    public int StepToGiveItems;
    public List<GameObject> ObjectsToGive = new();

    void Start()
    {
        GhostAnimator.gameObject.SetActive(false);
        if (!Enabled) return;
        StartCoroutine(ShowTutorial(InitialTime, InBetweenTime));
    }

    private void LateUpdate()
    {
        if(Ongoing)
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
}
