using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardInfoDisplay : MonoBehaviour
{
    // UI 更新用（必要に応じて Inspector から紐づけ）
    [Header("UI Elements")]
    [SerializeField]
    private Text titleText;
    [SerializeField] 
    private Image mainImage;
    [SerializeField]
    private Text intellectText;
    [SerializeField]
    private Text athleticismText;
    [SerializeField]
    private Text luckText;
    [SerializeField]
    private Text flavorText;
    
    
    public void DisplayCardData(GeminiCardGenerator.CardData data, string cardTitleText, Texture2D tex)
    {
        Debug.Log($"カード表示: 頭脳={data.intellect}, 運動={data.athleticism}, 運={data.luck}");
        Debug.Log($"フレーバーテキスト: {data.flavor_text}");

        // Inspector にセットした Text コンポーネントに出力
        if (titleText       != null) titleText.text       = cardTitleText;
        if (intellectText   != null) intellectText.text   = $"頭脳: {data.intellect}";
        if (athleticismText != null) athleticismText.text = $"運動: {data.athleticism}";
        if (luckText        != null) luckText.text        = $"運: {data.luck}";
        if (flavorText      != null) flavorText.text      = data.flavor_text;
        
        // Texture2D → Sprite に変換
        if (mainImage != null)
        {
            var sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f)
            );
            mainImage.sprite = sprite;
        }
    }
}
