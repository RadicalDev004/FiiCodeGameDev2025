using Pixelplacement;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public GameObject I_PressE, I_NotEnoughMana, I_Checkpoint, I_ZombiesAlive, I_Admin;
    public Image I_Hit, I_Blackout;
    public float fadeDuration = 1f;
    private Coroutine ShowHit;

    private CanvasGroup checkpointCanvasGroup;

    private bool adminOn = false;

    void Start()
    {
        checkpointCanvasGroup = I_Checkpoint.GetComponent<CanvasGroup>();
        if (checkpointCanvasGroup == null)
        {
            checkpointCanvasGroup = I_Checkpoint.AddComponent<CanvasGroup>();
        }
    }

    public void TogglePressE(bool state)
    {
        I_PressE.SetActive(state);
    }

    public void ToggleNotEnughMana(bool state)
    {
        I_NotEnoughMana.SetActive(state);
    }

    public void ToggleAdmin()
    {
        adminOn = !adminOn;

        if(adminOn)
            Cursor.lockState = CursorLockMode.None;
        else
            if(!Code.IsOpen && !Settings.IsOpen && !MapManager.IsOpen)
                Cursor.lockState = CursorLockMode.Locked;
            else 
                Cursor.lockState = CursorLockMode.None;

        I_Admin.SetActive(adminOn);
    }

    public void ToggleCheckpointText()
    {
        StartCoroutine(ShowCheckpointText());
    }

    public void ToggleZombiesAlive()
    {
        StartCoroutine(ShowZombiesAliveText());
    }

    IEnumerator ShowZombiesAliveText()
    {
        I_ZombiesAlive.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        I_ZombiesAlive.SetActive(false);
    }

    IEnumerator ShowCheckpointText()
    {
        I_Checkpoint.SetActive(true);

        yield return new WaitForEndOfFrame();

        I_Checkpoint.transform.GetChild(0).GetComponent<GlitchTextEffect>().StartGlitchEffect();
        I_Checkpoint.transform.GetChild(1).GetComponent<GlitchTextEffect>().StartGlitchEffect();

        yield return StartCoroutine(FadeCanvasGroup(checkpointCanvasGroup, 0, 1));

        yield return new WaitForSeconds(3f);

        yield return StartCoroutine(FadeCanvasGroup(checkpointCanvasGroup, 1, 0));

        I_Checkpoint.SetActive(false);
    }

    IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);  
            yield return null;
        }

        canvasGroup.alpha = endAlpha;  
    }

    public void OnHit()
    {
        if(ShowHit != null) StopCoroutine(ShowHit);

        ShowHit = StartCoroutine(OnHitCor(0.15f));
    }
    private IEnumerator OnHitCor(float duration)
    {
        Tween.Color(I_Hit, new Color32(255, 255, 255, 25), duration, 0, Tween.EaseInOut);
        yield return new WaitForSeconds(duration);
        Tween.Color(I_Hit, new Color32(255, 255, 255, 0), duration, 0, Tween.EaseInOut);
        yield return new WaitForSeconds(duration);
    }

    public void DoBlackOut(float time)
    {
        Tween.Color(I_Blackout, Color.black, time, 0, Tween.EaseIn);
    }
    public void RecoverBlackOut(float time)
    {
        Tween.Color(I_Blackout, Color.clear, time, 0, Tween.EaseIn);
    }
}
