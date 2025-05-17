using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro; // or UnityEngine.UI for standard Text

public class OpenAIChat : MonoBehaviour
{
    [Header("Set Your API Key Here")]
    private static readonly string part1 = "sk-proj-";
    private static readonly string part2 = "GB8GBCyw83bdLmsjWvhQKYjEb4yuo3UxkuAXkmVTrph3WSYt5j6vpfhLVlm6B32zOGWGyF_ffHT3BlbkFJg9nCvR2rsXgx_UR8OBLijHwF5Cw-1Kg6-7vgao4gCrtVnfbDcpmvfBNWucMGwdBDfye7dNs7AA";
    private static readonly string apiKey = part1 + part2;

    [Header("Optional UI")]
    public TMP_InputField inputField;
    public TMP_Text responseText;

    private const string apiUrl = "https://api.openai.com/v1/chat/completions";

    private void Start()
    {
        SendMessageToChat("Salut, ce faci?");
    }

    public void SendMessageToChat(string message)
    {
        StartCoroutine(SendChatRequest(message));
    }
    
    IEnumerator SendChatRequest(string userInput)
    {
        // Construct request payload
        string jsonBody = JsonUtility.ToJson(new ChatRequest(userInput));

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
            responseText.text = $"Error: {request.error}";
        }
        else
        {
            string jsonResult = request.downloadHandler.text;
            var reply = ParseResponse(jsonResult);
            Debug.Log(reply);
        }
    }

    string ParseResponse(string json)
    {
        var wrapper = JsonUtility.FromJson<ChatResponseWrapper>(json);
        return wrapper?.choices?[0]?.message?.content?.Trim() ?? "No reply.";
    }

    // Helper classes
    [System.Serializable]
    public class ChatRequest
    {
        public string model = "gpt-4.1";
        public Message[] messages;

        public ChatRequest(string userMessage)
        {
            messages = new[] { new Message { role = "user", content = userMessage } };
        }
    }

    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    public class ChatResponseWrapper
    {
        public Choice[] choices;
    }

    [System.Serializable]
    public class Choice
    {
        public Message message;
    }
}
