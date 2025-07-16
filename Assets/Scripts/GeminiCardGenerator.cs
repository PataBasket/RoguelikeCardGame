using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO; // ファイル操作用
using System; // Convert.ToBase64String用

public class GeminiCardGenerator : MonoBehaviour
{
    // Gemini APIキーをここに設定してください。
    // 注意: 本番環境では、APIキーをコードに直接埋め込むのは避けるべきです。
    // 環境変数や安全な設定ファイルから読み込むことを検討してください。
    [SerializeField]
    private string geminiApiKey = "YOUR_GEMINI_API_KEY";

    // Gemini APIのエンドポイント
    private const string GeminiApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-pro:generateContent?key="; // Gemini 2.5 Proを使用

    // Gemini APIに送信するプロンプト
    [TextArea(3, 10)]
    public string geminiPrompt = @"
以下の写真に写っている人またはペットの特徴を読み取り、カードゲームのパラメータとして「頭脳」「運動」「運」の3つの数値をそれぞれ1から100の範囲で評価してください。各数値は整数でお願いします。また、そのキャラクターに合う1、2文の短いフレーバーテキストも作成してください。

出力はJSON形式でお願いします。JSONのキーは以下の通りにしてください:
- 頭脳: `intellect`
- 運動: `athleticism`
- 運: `luck`
- フレーバーテキスト: `flavor_text`

例:
```json
{
  ""intellect"": 85,
  ""athleticism"": 70,
  ""luck"": 60,
  ""flavor_text"": ""鋭い洞察力で戦場を支配し、困難な状況も乗り越える賢者。""
}
```
";

    // カードデータを受け取るためのイベント
    public event Action<CardData> OnCardDataGenerated;

    // --- 公開メソッド ---

    /// <summary>
    /// Resourcesフォルダ内の指定された画像名から画像を読み込み、Gemini APIに送信してカードデータを生成します。
    /// 画像はResourcesフォルダの直下、またはサブフォルダ内に配置されている必要があります。
    /// 例: "MyImages/pet_photo" (拡張子なし)
    /// </summary>
    /// <param name="resourceName">Resourcesフォルダ内の画像名 (拡張子なし)</param>
    public void GenerateCardDataFromResourceImage(string resourceName)
    {
        // Resources.Loadを使用してTexture2Dを読み込む
        Texture2D texture = Resources.Load<Texture2D>(resourceName);

        if (texture == null)
        {
            Debug.LogError($"Resourcesフォルダ内に画像 '{resourceName}' が見つかりませんでした。拡張子なしで指定してください。");
            return;
        }

        // Texture2DをBase64文字列に変換
        // ここでは常にJPEGとしてエンコードします。
        string base64Image = Texture2DToBase64(texture, out string mimeType);
        if (string.IsNullOrEmpty(base64Image))
        {
            // エラーはTexture2DToBase64内でログ出力されるため、ここでは追加のログは不要
            return;
        }

        // Gemini APIリクエストを開始
        StartCoroutine(SendToGeminiApi(base64Image, mimeType, geminiPrompt));
    }

    // --- 内部処理メソッド ---

    /// <summary>
    /// Texture2DをJPEG形式のバイト配列にエンコードし、Base64文字列に変換します。
    /// </summary>
    /// <param name="texture">変換するTexture2Dオブジェクト</param>
    /// <param name="mimeType">出力される画像のMIMEタイプ (常に "image/jpeg")</param>
    /// <returns>Base64エンコードされた画像データ文字列</returns>
    private string Texture2DToBase64(Texture2D texture, out string mimeType)
    {
        mimeType = "image/jpeg"; // JPEG形式でエンコードするため、MIMEタイプを固定

        if (!texture.isReadable)
        {
            Debug.LogError($"Texture2D '{texture.name}' は読み取り可能ではありません。Inspectorで 'Read/Write Enabled' を有効にしてください。");
            return null;
        }

        try
        {
            // Texture2DをJPEG形式のバイト配列にエンコード (品質はデフォルトで75%)
            byte[] imageBytes = texture.EncodeToJPG();
            
            // 画像サイズをログ出力して確認
            Debug.Log($"エンコードされた画像のサイズ: {imageBytes.Length / 1024} KB ({texture.width}x{texture.height}px)");

            return Convert.ToBase64String(imageBytes);
        }
        catch (Exception e)
        {
            Debug.LogError($"Texture2DをBase64に変換中にエラーが発生しました: {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// Gemini APIにHTTPリクエストを送信し、レスポンスを処理します。
    /// </summary>
    /// <param name="base64Image">Base64エンコードされた画像データ</param>
    /// <param name="mimeType">画像のMIMEタイプ (例: "image/png", "image/jpeg")</param>
    /// <param name="prompt">Geminiに送信するテキストプロンプト</param>
    /// <returns>コルーチン</returns>
    private IEnumerator SendToGeminiApi(string base64Image, string mimeType, string prompt)
    {
        string fullUrl = GeminiApiUrl + geminiApiKey;

        // InlineDataとGenerationConfigをJsonUtilityでシリアライズ
        string inlineDataJson = JsonUtility.ToJson(new InlineData { mime_type = mimeType, data = base64Image });
        // maxOutputTokensを増やして、モデルが完全なJSONを生成できるようにする
        string generationConfigJson = JsonUtility.ToJson(new GenerationConfig { temperature = 0.7f, maxOutputTokens = 2048 });

        // 'parts'配列のJSONを手動で構築
        // これにより、各Partオブジェクトが'text'または'inline_data'のどちらか一方のみを含むようにし、
        // 'oneof'エラーを解決します。
        string partsJson = $"[{{\"text\": \"{EscapeJsonString(prompt)}\"}}, {{\"inline_data\": {inlineDataJson}}}]";

        // リクエストボディ全体のJSON文字列を構築
        string requestBodyJson = $"{{\"contents\": [{{\"parts\": {partsJson}}}], \"generationConfig\": {generationConfigJson}}}";

        // 構築されたJSONをデバッグログに出力して確認
        Debug.Log("Constructed Request JSON: " + requestBodyJson);

        using (UnityWebRequest request = new UnityWebRequest(fullUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestBodyJson);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("Gemini APIにリクエストを送信中...");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log("Gemini APIレスポンス: " + responseText);
                ParseGeminiResponse(responseText);
            }
            else
            {
                Debug.LogError($"Gemini APIエラー: {request.error}");
                Debug.LogError($"レスポンスコード: {request.responseCode}");
                Debug.LogError($"エラー詳細: {request.downloadHandler.text}"); // ここにAPIからの詳細なエラーメッセージが含まれることが多い
            }
        }
    }

    /// <summary>
    /// JSON文字列内の特殊文字をエスケープするためのヘルパーメソッド。
    /// </summary>
    /// <param name="s">エスケープする文字列</param>
    /// <returns>エスケープされた文字列</returns>
    private string EscapeJsonString(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return "";
        }
        // 基本的なJSONエスケープ。より複雑な文字列にはさらに包括的なエスケープが必要になる場合があります。
        return s.Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t")
                .Replace("/", "\\/"); // URLやパスの場合に推奨
    }

    /// <summary>
    /// Gemini APIからのJSONレスポンスをパースし、カードデータを抽出します。
    /// </summary>
    /// <param name="jsonResponse">Gemini APIからの生JSONレスポンス</param>
    private void ParseGeminiResponse(string jsonResponse)
    {
        try
        {
            GeminiResponse apiResponse = JsonUtility.FromJson<GeminiResponse>(jsonResponse);

            if (apiResponse != null && apiResponse.candidates != null && apiResponse.candidates.Length > 0)
            {
                // content.parts が存在するかどうかを確認
                if (apiResponse.candidates[0].content != null && 
                    apiResponse.candidates[0].content.parts != null && 
                    apiResponse.candidates[0].content.parts.Length > 0)
                {
                    string generatedText = apiResponse.candidates[0].content.parts[0].text;
                    Debug.Log("Geminiが生成したテキスト: " + generatedText);

                    // GeminiはJSONをMarkdownコードブロックで返すことがあるため、それを除去します。
                    string cleanedJson = generatedText.Replace("```json", "").Replace("```", "").Trim();

                    CardData cardData = JsonUtility.FromJson<CardData>(cleanedJson);

                    Debug.Log($"カードデータ生成成功: 頭脳={cardData.intellect}, 運動={cardData.athleticism}, 運={cardData.luck}, フレーバーテキスト='{cardData.flavor_text}'");

                    // 生成されたカードデータをイベントで通知
                    OnCardDataGenerated?.Invoke(cardData);
                }
                else
                {
                    Debug.LogError("Gemini APIレスポンスに生成されたテキストコンテンツが見つかりませんでした。");
                    Debug.LogError("レスポンス詳細: " + jsonResponse);
                }
            }
            else
            {
                Debug.LogError("Gemini APIレスポンスに有効な候補が見つかりませんでした。");
                Debug.LogError("レスポンス詳細: " + jsonResponse);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Gemini APIレスポンスのパース中にエラーが発生しました: {e.Message}");
            Debug.LogError($"パースできなかったJSON: {jsonResponse}");
        }
    }

    // --- JSONシリアライズ/デシリアライズ用クラス ---

    // Gemini APIリクエストボディの構造 (手動構築のため、これらのクラスは直接シリアライズには使用されません)
    // ただし、JsonUtility.ToJsonで部分的に使用されるInlineDataやGenerationConfigは保持します。

    [System.Serializable]
    public class InlineData
    {
        public string mime_type;
        public string data;
    }

    [System.Serializable]
    public class GenerationConfig
    {
        public float temperature;
        public int maxOutputTokens;
    }

    // Gemini APIレスポンスのルート構造
    [System.Serializable]
    public class GeminiResponse
    {
        public Candidate[] candidates;
    }

    [System.Serializable]
    public class Candidate
    {
        public Content content;
        // 他にもsafetyRatingsなどがありますが、今回は省略
    }

    [System.Serializable]
    public class Content
    {
        public Part[] parts; // レスポンスパース用には必要
    }

    [System.Serializable]
    public class Part
    {
        public string text; // レスポンスパース用には必要
        // inline_dataはレスポンスには含まれないため、ここでは不要
    }

    // Geminiが生成するカードデータの構造
    [System.Serializable]
    public class CardData
    {
        public int intellect;
        public int athleticism;
        public int luck;
        public string flavor_text;
    }
}
