using UnityEngine;
using TMPro;

public class TypingSoundController : MonoBehaviour
{
    private TMP_InputField inputField;

    private string previousText = "";

    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.onValueChanged.AddListener(OnTextChanged);
        previousText = inputField.text;
    }

    void OnTextChanged(string currentText)
    {
        if (currentText.Length > previousText.Length)
        {
            AudioManager.Play("Code_Type");
        }
        else if (currentText.Length < previousText.Length)
        {
            AudioManager.Play("Code_Delete");
        }

        previousText = currentText;
    }
}
