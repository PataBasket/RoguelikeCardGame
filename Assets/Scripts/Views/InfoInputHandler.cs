using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoInputHandler : MonoBehaviour
{
    [SerializeField] private GameObject imageSelectionPanel;
    [SerializeField] private GameObject cardGeneratingPanel;
    [SerializeField] private GameObject cardResultPanel;

    [SerializeField] private Text cardTitle;
    [SerializeField] private Text cardExplanation;
    [SerializeField] private Image imageChosen;

    private ImageExtractor _imageExtractor;
    
    // Start is called before the first frame update
    void Start()
    {
        _imageExtractor = new ImageExtractor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // 「写真を撮る」ボタンが押されたら実行される
    public void OnClickTakePicture()
    {
        
    }
    
    //　「画像を選択」ボタンが押されたら実行される
    public void OnClickChoosePicture()
    {
        _imageExtractor.ExtractImage(
            texture =>
            {
                // Texture2D → Sprite に変換
                var sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );
                imageChosen.sprite = sprite;

                // 必要に応じて次のパネル切り替えなど
                // imageSelectionPanel.SetActive(false);
                // cardGeneratingPanel.SetActive(true);
            },
            errorMsg =>
            {
                Debug.LogError(errorMsg);
                // ここでUI上にエラーメッセージを出してもOK
                // 例：専用の Text を用意して errorMsg を表示する
            }
        );
    }

    // 「この内容で作成する」ボタンが押されたら実行される
    public void OnClickSubmitCard()
    {
        
    } 
}
