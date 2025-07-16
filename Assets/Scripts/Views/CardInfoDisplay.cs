using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInfoDisplay : MonoBehaviour
{
    [SerializeField]
    private GeminiCardGenerator generator;

    void Start()
    {
        generator.GenerateCardDataFromResourceImage("testPhoto");
        if (generator != null)
        {
            generator.OnCardDataGenerated += DisplayCardData;
        }
    }

    private void DisplayCardData(GeminiCardGenerator.CardData data)
    {
        Debug.Log($"カード表示: 頭脳: {data.intellect}, 運動: {data.athleticism}, 運: {data.luck}");
        Debug.Log($"フレーバーテキスト: {data.flavor_text}");
        // ここでUI要素（Text、Imageなど）を更新してカードを表示します。
    }

    void OnDestroy()
    {
        // イベント購読の解除を忘れずに行う
        GeminiCardGenerator generator = FindObjectOfType<GeminiCardGenerator>();
        if (generator != null)
        {
            generator.OnCardDataGenerated -= DisplayCardData;
        }
    }
}
