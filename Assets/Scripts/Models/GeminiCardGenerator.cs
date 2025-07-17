using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System;
using System.Text;

public class GeminiCardGenerator
{
    private string _apiKey;
    private string ApiUrl => $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";


    public GeminiCardGenerator()
    {
        LoadApiKey();
    }

    [System.Serializable]
    public class ApiConfig
    {
        public string apiKey;
    }
    
    private void LoadApiKey()
    {
        // Resourcesフォルダ内のApiConfig.jsonファイルを読み込む
        TextAsset configFile = Resources.Load<TextAsset>("ApiConfig");
        if (configFile != null)
        {
            var config = JsonUtility.FromJson<ApiConfig>(configFile.text);
            _apiKey = config.apiKey;
        }
        else
        {
            Debug.LogError("ApiConfig.json not found in Resources folder.");
        }
    }

    public async UniTask<CardData> GenerateCardDataAsync(Texture2D texture, string prompt)
    {
        // 1. 画像→Base64
        if (!texture.isReadable)
            Debug.LogError($"Texture '{texture.name}' が Read/Write できません。");
        byte[] jpg = texture.EncodeToJPG();
        string base64 = Convert.ToBase64String(jpg);

        // 2. inline_data と generationConfig を文字列化
        string inlineDataJson = JsonUtility.ToJson(new InlineData {
            mime_type = "image/jpeg",
            data      = base64
        });
        string generationConfigJson = JsonUtility.ToJson(new GenerationConfig {
            temperature     = 0.7f,
            maxOutputTokens = 2048
        });

        // 3. parts 配列を完全手動組み立て
        string partsJson = "["
            + "{\"text\":\""      + EscapeJsonString(prompt) + "\"},"
            + "{\"inline_data\":" + inlineDataJson          + "}"
            + "]";

        // 4. 本体リクエスト JSON を手動組み立て
        string requestJson = "{"
            + "\"contents\":[{\"parts\":"     + partsJson            + "}],"
            + "\"generationConfig\":"          + generationConfigJson
            + "}";

        Debug.Log("Request JSON:\n" + requestJson);

        // 5. リクエスト送信
        using var req = new UnityWebRequest(ApiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(requestJson);
        req.uploadHandler   = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        await req.SendWebRequest().ToUniTask();
        if (req.result != UnityWebRequest.Result.Success)
            throw new Exception($"Gemini API エラー: {req.error}");

        // 6. レスポンスの受け取り＆パース
        string responseText = req.downloadHandler.text;
        Debug.Log("Raw Response:\n" + responseText);

        var apiResp = JsonUtility.FromJson<GeminiResponse>(responseText);
        string generated = apiResp.candidates[0].content.parts[0].text;
        string cleaned   = generated
            .Replace("```json", "")
            .Replace("```", "")
            .Trim();
        Debug.Log("Cleaned JSON:\n" + cleaned);

        var card = JsonUtility.FromJson<CardData>(cleaned);
        Debug.Log($"Parsed → 頭脳:{card.intellect} 運動:{card.athleticism} 運:{card.luck} テキスト:'{card.flavor_text}'");
        return card;
    }

    private string EscapeJsonString(string s)
    {
        return s.Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
    }

    [Serializable]
    public class InlineData
    {
        public string mime_type;
        public string data;
    }

    [Serializable]
    public class GenerationConfig
    {
        public float temperature;
        public int   maxOutputTokens;
    }

    [Serializable]
    public class GeminiResponse
    {
        public Candidate[] candidates;
    }
    [Serializable]
    public class Candidate
    {
        public Content content;
    }
    [Serializable]
    public class Content
    {
        public Part[] parts;
    }
    [Serializable]
    public class Part
    {
        public string text;
    }

    [Serializable]
    public class CardData
    {
        public int    intellect;
        public int    athleticism;
        public int    luck;
        public string flavor_text;
    }
}
