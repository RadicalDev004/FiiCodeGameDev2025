using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PuzzleCompleteEffect : MonoBehaviour
{
    public static PuzzleCompleteEffect Instance;

    public GameObject glyphPrefab;
    public Transform whereTo;
    public string message;
    public float letterSpacing = 40f;
    public float spawnDelay = 0.05f;

    private List<GameObject> glyphs = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    public void StartAnimation()
    {
        StartCoroutine(SpawnLetters());
    }

    IEnumerator SpawnLetters()
    {
        //Debug.Log("am inceput");
        int visibleCharCount = 0;
        foreach (char c in message)
        {
            if (c != ' ') visibleCharCount++;
        }

        float startX = -(visibleCharCount - 1) / 2f * letterSpacing;
        int letterIndex = 0;

        for (int i = 0; i < message.Length; i++)
        {
            char c = message[i];
            if (c == ' ') continue;

            Vector3 spawnPos = new Vector3(startX + letterIndex * letterSpacing, 0, 0);
            GameObject letterObj = Instantiate(glyphPrefab, whereTo);
            letterObj.GetComponent<RectTransform>().anchoredPosition = spawnPos;

            TextMeshProUGUI tmp = letterObj.GetComponent<TextMeshProUGUI>();
            tmp.text = c.ToString();

            glyphs.Add(letterObj);
            letterIndex++;

            //Debug.Log(c.ToString());

            yield return new WaitForSeconds(spawnDelay);
        }

        yield return new WaitForSeconds(2f);
        foreach (GameObject glyph in glyphs)
        {
            if (glyph != null)
                StartCoroutine(GlitchAndFlyAway(glyph));
        }
        glyphs.Clear(); 


        yield return new WaitForSeconds(2f);
        //gameObject.SetActive(false);
    }

    IEnumerator GlitchAndFlyAway(GameObject glyph)
    {
        if (glyph == null) yield break;

        TextMeshProUGUI tmp = glyph.GetComponent<TextMeshProUGUI>();
        if (tmp == null) yield break;

        string original = tmp.text;

        for (int i = 0; i < 5; i++)
        {
            if (tmp == null) yield break;
            tmp.text = GetRandomCharacter().ToString();
            tmp.color = Random.ColorHSV();
            yield return new WaitForSeconds(0.05f);
        }

        if (tmp != null) tmp.text = original;

        RectTransform rt = glyph.GetComponent<RectTransform>();
        if (rt == null) yield break;

        Vector2 direction = Random.insideUnitCircle.normalized * Random.Range(100f, 200f);

        float duration = 1f;
        float elapsed = 0f;
        Vector2 startPos = rt.anchoredPosition;
        Vector3 startScale = rt.localScale;

        while (elapsed < duration)
        {
            if (rt == null || tmp == null) yield break;

            rt.anchoredPosition = Vector2.Lerp(startPos, startPos + direction, elapsed / duration);
            rt.rotation = Quaternion.Euler(0, 0, elapsed * 360f);
            tmp.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            rt.localScale = Vector3.Lerp(startScale, Vector3.zero, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (glyph != null)
            Destroy(glyph);
    }



    char GetRandomCharacter()
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@#%&!";
        return chars[Random.Range(0, chars.Length)];
    }
}
