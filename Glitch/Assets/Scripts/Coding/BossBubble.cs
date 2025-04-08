using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBubble : Editable
{
    public int MaximumGlitches = 15;
    public Vector3 InitialPos;
    public Transform AnimationPos;
    public Vector3 MovePlayerTo;
    public float AnimationTime = 5;
    public ParticleSystem OnAngry;
    public GameObject Book1, Book2;
    private void Awake()
    {
        InitialPos = transform.position;
        ValidateCode = Validate;
        GenerateCode();
    }

    public bool Validate(List<string> code)
    {
        if (code.Count != 1)
        {
            Debug.LogError("Failed validation at length " + code.Count);
            return false;
        }
        if (code[0] != "false" && code[0] != "true")
        {
            Debug.LogError("Failed validation at type " + code[0]);
            return false;
        }

        if (code[0] == "true" && PlayerBehaviour.GlitchesSolved < MaximumGlitches)
        {
            Debug.LogError("Failed validation at not completed all glitches " + code[0]);
            return false;
        }

        Tween.LocalPosition(transform, AnimationPos.localPosition, AnimationTime, 0, Tween.EaseInOut, obeyTimescale: true);
        StartCoroutine(MovePlayerCor(AnimationTime));

        ToggleOutline(false);
        Completed = true;

        return true;
    }

    public void GenerateCode()
    {
        ExecutableCode = $"\r\n<color=white>resolved_glitches = {PlayerBehaviour.GlitchesSolved};</color>"
            + "\r\n\r\ntake_to_final_boss = <e>false</e>;"
            + "\r\n\r\n<color=#44cd8b>/* you can try to defeat the evil book only after you have resolved all glitches */</color>\r\n";
    }

    private IEnumerator MovePlayerCor(float time)
    {
        Ref.PlayerBehaviour.GetComponent<CharacterController>().enabled = false;
        Movement.isPaused = true;
        Ref.MagicBook.StopLookingAway = true;

        Ref.PlayerBehaviour.transform.SetParent(transform);
        Ref.PlayerBehaviour.transform.localPosition = MovePlayerTo;

        yield return new WaitForSeconds(time);

        Ref.PlayerBehaviour.GetComponent<CharacterController>().enabled = true;
        Movement.isPaused = false;
        Ref.PlayerBehaviour.transform.SetParent(null);
        Ref.MagicBook.GetAngry();
        OnAngry.Play();

        yield return new WaitForSeconds(3);

        Ref.UI.DoBlackOut(1);
        yield return new WaitForSeconds(1);

        Ref.PlayerBehaviour.GetComponent<CharacterController>().enabled = false;
        Movement.isPaused = true;
        LookPC.isPaused = true;

        Ref.PlayerBehaviour.transform.position = new Vector3(-3.3f, -1f, -0.6f);
        Ref.PlayerBehaviour.playerCamera.transform.LookAt(Book2.transform);
        Book1.SetActive(false); Book2.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        Ref.PlayerBehaviour.GetComponent<CharacterController>().enabled = true;
        Movement.isPaused = false;
        LookPC.isPaused = false;
        Ref.UI.RecoverBlackOut(0.5f);
    }

    public void ResetState()
    {
        if(!Completed) return;

        Book1.GetComponent<MagicBook>().StopLookingAway = false;
        Book1.SetActive(true);
        Book2.GetComponent<FinalBoss>().ResetState();
        Book2.SetActive(false);
        transform.position = InitialPos;
        Completed = false;
        GenerateCode();
    }
}
