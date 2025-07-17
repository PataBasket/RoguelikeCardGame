using UnityEngine;
using Cysharp.Threading.Tasks;  // UniTask
using System;

public class CardInfoDisplay : MonoBehaviour
{
    [Header("Gemini 設定")]
    [SerializeField]
    private string geminiApiKey = "YOUR_GEMINI_API_KEY";

    [Header("カード生成")]
    [SerializeField]
    private string resourceImageName = "testPhoto";    // Resources から読み込む画像名

    // UI 更新用（必要に応じて Inspector から紐づけ）
    [Header("UI Elements")]
    [SerializeField]
    private UnityEngine.UI.Text intellectText;
    [SerializeField]
    private UnityEngine.UI.Text athleticismText;
    [SerializeField]
    private UnityEngine.UI.Text luckText;
    [SerializeField]
    private UnityEngine.UI.Text flavorText;

    private GeminiCardGenerator _geminiCardGenerator;
    
    private string _geminiPrompt = @"
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

    // Start を UniTask に置き換え
    async UniTaskVoid Start()
    {
        // サービス初期化
        _geminiCardGenerator = new GeminiCardGenerator(geminiApiKey);

        // Resources から画像読み込み
        Texture2D tex = Resources.Load<Texture2D>(resourceImageName);
        if (tex == null)
        {
            Debug.LogError($"画像 '{resourceImageName}' が Resources 内に見つかりません。");
            return;
        }

        try
        {
            // カードデータ生成を待機
            var card = await _geminiCardGenerator.GenerateCardDataAsync(tex, _geminiPrompt);

            // UI 更新
            DisplayCardData(card);
        }
        catch (Exception ex)
        {
            Debug.LogError("カード生成に失敗: " + ex.Message);
        }
    }

    private void DisplayCardData(GeminiCardGenerator.CardData data)
    {
        Debug.Log($"カード表示: 頭脳={data.intellect}, 運動={data.athleticism}, 運={data.luck}");
        Debug.Log($"フレーバーテキスト: {data.flavor_text}");

        // Inspector にセットした Text コンポーネントに出力
        if (intellectText   != null) intellectText.text   = $"頭脳: {data.intellect}";
        if (athleticismText != null) athleticismText.text = $"運動: {data.athleticism}";
        if (luckText        != null) luckText.text        = $"運: {data.luck}";
        if (flavorText      != null) flavorText.text      = data.flavor_text;
    }
}
