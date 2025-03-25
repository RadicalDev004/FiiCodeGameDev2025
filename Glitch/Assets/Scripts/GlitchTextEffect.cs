using System.Collections;
using UnityEngine;
using TMPro;

public class GlitchTextEffect : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float glitchFrequency = 0.2f;
    public float shakeIntensity = 1f;
    public int lettersToChange = 1;
    public float colorChangeChance = 0.5f;

    private string originalText;

    private Coroutine glitchCoroutine;

    void Start()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<TextMeshProUGUI>();
        }
        originalText = textComponent.text;
    }

    public void StartGlitchEffect()
    {
        if (glitchCoroutine != null)
        {
            StopCoroutine(glitchCoroutine);
        }

        glitchCoroutine = StartCoroutine(GlitchEffect());
    }

    IEnumerator GlitchEffect()
    {
        while (true)
        {
            char[] glitchedText = originalText.ToCharArray();
            string finalText = "";

            for (int i = 0; i < originalText.Length; i++)
            {
                if (Random.value < (float)lettersToChange / originalText.Length)
                {
                    glitchedText[i] = GetRandomCharacter();
                }

                string charColor = (Random.value < colorChangeChance) ? GetRandomColor() : "#44C951";
                finalText += $"<color={charColor}>{glitchedText[i]}</color>";
            }

            textComponent.text = finalText;

            Vector2 originalPos = textComponent.rectTransform.anchoredPosition;
            textComponent.rectTransform.anchoredPosition = originalPos + new Vector2(
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity)
            );

            yield return new WaitForSeconds(glitchFrequency);
        }
    }

    char GetRandomCharacter()
    {
        string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
        return characters[Random.Range(0, characters.Length)];
    }

    string GetRandomColor()
    {
        Color randomColor = new Color(Random.value, Random.value, Random.value, 0.5019608f);
        return $"#{ColorUtility.ToHtmlStringRGB(randomColor)}";
    }
}
