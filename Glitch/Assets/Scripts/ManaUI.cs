using Pixelplacement;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ManaUI : MonoBehaviour
{
    public GameObject manaBar;

    private void Start()
    {
        if (ManaSystem.Instance != null)
        {
            ManaSystem.Instance.OnManaChanged += UpdateManaUI;
        }
        else
        {
            Debug.LogError("Mana system null");
        }
    }

    private void UpdateManaUI(float manaPercent)
    {
        Debug.LogError("Updating mana to " + manaPercent);
        StartCoroutine(ChangeManaUIAnimation(manaPercent, 1));
    }

    private IEnumerator ChangeManaUIAnimation(float manaPercent, float duration)
    {
        float elapsedTime = 0f;
        float startValue = manaBar.GetComponent<Image>().fillAmount;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            manaBar.GetComponent<Image>().fillAmount = Mathf.Lerp(startValue, manaPercent, elapsedTime / duration);
            yield return null;
        }

        manaBar.GetComponent<Image>().fillAmount = manaPercent;
    }

    private void OnDestroy()
    {
        ManaSystem.Instance.OnManaChanged -= UpdateManaUI;
    }
}
