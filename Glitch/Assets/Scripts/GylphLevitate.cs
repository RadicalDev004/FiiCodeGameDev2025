using System.Collections;
using UnityEngine;
using TMPro;

public class GlyphLevitate : MonoBehaviour
{
    public float amplitudeY = 5f;
    public float frequencyY = 2f;
    public float amplitudeX = 3f;
    public float frequencyX = 1.5f;
    public float glitchInterval = 1.5f;
    public float glitchDuration = 0.2f;

    private Vector3 startPos;
    private TextMeshProUGUI tmp;
    private string originalText;
    private bool isGlitching = false;

    void Start()
    {
        startPos = transform.localPosition;
        tmp = GetComponent<TextMeshProUGUI>();
        originalText = tmp.text;

        StartCoroutine(GlitchLoop());
    }

    void Update()
    {
        float offsetY = Mathf.Sin(Time.time * frequencyY + GetInstanceID() % 10) * amplitudeY;
        float offsetX = Mathf.Cos(Time.time * frequencyX + GetInstanceID() % 10) * amplitudeX;

        transform.localPosition = startPos + new Vector3(offsetX, offsetY, 0);

        float alpha = 0.7f + Mathf.Sin(Time.time * 5f) * 0.2f;
        Color currentColor = tmp.color;
        tmp.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
    }

    IEnumerator GlitchLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(glitchInterval * 0.5f, glitchInterval * 1.5f));
            if (!isGlitching)
                StartCoroutine(DoGlitch());
        }
    }

    IEnumerator DoGlitch()
    {
        isGlitching = true;

        char randomChar = GetRandomCharacter();
        string randomColor = GetRandomColor();
        tmp.text = $"<color={randomColor}>{randomChar}</color>";

        yield return new WaitForSeconds(glitchDuration);

        tmp.text = originalText;
        isGlitching = false;
    }

    char GetRandomCharacter()
    {
        string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
        return characters[Random.Range(0, characters.Length)];
    }

    string GetRandomColor()
    {
        Color randomColor = new Color(Random.value, Random.value, Random.value);
        return $"#{ColorUtility.ToHtmlStringRGB(randomColor)}";
    }
}
