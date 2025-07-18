using UnityEngine;
using Cysharp.Threading.Tasks;  // UniTask
using System;

public class PromptFormatter
{
    private string _cardTitle;
    private string _cardExplanation;
    
    private GeminiCardGenerator _geminiCardGenerator;
    
    private string _geminiPrompt => $@"
        以下の写真に写っている人またはペットの特徴を読み取り、カードゲームのパラメータとして「頭脳」「運動」「運」の3つの数値をそれぞれ1から100の範囲で評価してください。三つのパラメータの合計値は100とし、各数値は整数でお願いします。また、そのキャラクターに合う1、2文の短いフレーバーテキストも作成してください。
        
        ※ カードタイトル：「{_cardTitle}」
        ※ 説明文：「{_cardExplanation}」

        出力はJSON形式でお願いします。JSONのキーは以下の通りにしてください:
        - 頭脳: `intellect`
        - 運動: `athleticism`
        - 運: `luck`
        - フレーバーテキスト: `flavor_text`

        例:
        ```json
        {{
          ""intellect"": 50,
          ""athleticism"": 40,
          ""luck"": 10,
          ""flavor_text"": ""鋭い洞察力で戦場を支配し、困難な状況も乗り越える賢者。""
        }}
        ```
        ";

    private Texture2D _tex = new Texture2D(2,2);

    public PromptFormatter()
    {
        _geminiCardGenerator = new GeminiCardGenerator();
    }

    // Start を UniTask に置き換え
    public async UniTaskVoid OrganizePrompt(string cardTitle, string cardExplanation, Texture2D tex)
    {
        _tex = tex;
        _cardTitle = cardTitle;
        _cardExplanation = cardExplanation;
        
        if (_tex == null)
        {
            Debug.LogError("画像が見つかりません。");
            return;
        }

        try
        {
            // カードデータ生成を待機
            var card = await _geminiCardGenerator.GenerateCardDataAsync(_tex, _geminiPrompt);

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
    }
}
