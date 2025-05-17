using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro; // or UnityEngine.UI for standard Text
using System;
using System.Collections.Generic;

public class OpenAIChat : MonoBehaviour
{
    [Header("Set Your API Key Here")]
    private static readonly string part1 = "sk-proj-";
    private static readonly string part2 = "GB8GBCyw83bdLmsjWvhQKYjEb4yuo3UxkuAXkmVTrph3WSYt5j6vpfhLVlm6B32zOGWGyF_ffHT3BlbkFJg9nCvR2rsXgx_UR8OBLijHwF5Cw-1Kg6-7vgao4gCrtVnfbDcpmvfBNWucMGwdBDfye7dNs7AA";
    private static readonly string apiKey = part1 + part2;
    private List<Message> messageHistory;

    [Header("Optional UI")]
    public TMP_InputField inputField;
    public TMP_Text responseText;

    public static OpenAIChat Instance;

    private const string apiUrl = "https://api.openai.com/v1/chat/completions";

    private void Start()
    {
        if (Instance == null)
            Instance = this;
    }

    public static void RequestChat(string message, Action<string> callback)
    {
        Instance.StartCoroutine(Instance.SendChatRequest(message, callback));
    }

    public static void RequestChatWithHistory(List<Message> messageHistory, Action<string> callback)
    {
        Instance.StartCoroutine(Instance.SendChatRequestWithHistory(messageHistory, callback));
    }

    public static void SendUserMessage(string userInput, Action<string> callback)
    {
        Instance.messageHistory.Add(new Message { role = "user", content = userInput });
        RequestChatWithHistory(new List<Message>(Instance.messageHistory), response =>
        {
            Instance.messageHistory.Add(new Message { role = "assistant", content = response });

            callback?.Invoke(response);
        });
    }


    IEnumerator SendChatRequest(string userInput, Action<string> callback)
    {
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
            callback?.Invoke(reply);
        }
    }

    IEnumerator SendChatRequestWithHistory(List<Message> messages, Action<string> callback)
    {
        var requestObj = new ChatRequest(messages);
        string jsonBody = JsonUtility.ToJson(requestObj);

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
            if (responseText != null)
                responseText.text = $"Error: {request.error}";
        }
        else
        {
            string jsonResult = request.downloadHandler.text;
            var reply = ParseResponse(jsonResult);
            callback?.Invoke(reply);
        }
    }

    public void ResetChatHistory(string systemPrompt = null)
    {
        messageHistory.Clear();
        if (!string.IsNullOrEmpty(systemPrompt))
        {
            messageHistory.Add(new Message
            {
                role = "system",
                content = systemPrompt
            });
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
        public List<Message> messages;

        public ChatRequest(string singleMessage)
        {
            messages = new List<Message>
        {
            new Message { role = "user", content = singleMessage }
        };
        }

        public ChatRequest(List<Message> messages)
        {
            this.messages = messages;
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