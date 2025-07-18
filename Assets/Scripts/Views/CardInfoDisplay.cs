using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInfoDisplay : MonoBehaviour
{
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
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void DisplayCardData(GeminiCardGenerator.CardData data)
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
