using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoInputHandler : MonoBehaviour
{
    [SerializeField] private GameObject imageSelectionPanel;
    [SerializeField] private GameObject cardGeneratingPanel;
    [SerializeField] private GameObject cardResultPanel;

    [SerializeField] private Text cardTitleText;
    [SerializeField] private Text cardExplanationText;
    [SerializeField] private Image imageChosen;

    [SerializeField] private Text errorMessage;

    private ImageExtractor _imageExtractor;
    private PromptFormatter _promptFormatter;
    private CardInfoDisplay _cardInfoDisplay;
    private CardController _cardController;
    
    // Start is called before the first frame update
    void Start()
    {
        _imageExtractor = new ImageExtractor();
        _promptFormatter = new PromptFormatter();
        _cardInfoDisplay = GetComponent<CardInfoDisplay>();
        _cardController = GetComponent<CardController>();
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
                errorMessage.gameObject.SetActive(false);
                // Texture2D → Sprite に変換
                var sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );
                imageChosen.sprite = sprite;
            },
            errorMsg =>
            {
                Debug.LogError(errorMsg);
                errorMessage.gameObject.SetActive(true);
                errorMessage.text = errorMsg;
            }
        );
    }

    // 「この内容で作成する」ボタンが押されたら実行される
    public async void OnClickSubmitCard()
    {
        SoundManager.Instance?.PlayBGM(SoundManager.BGMData.BGMTYPE.CardCreate);
        SoundManager.Instance?.PlaySE(SoundManager.SEData.SETYPE.CardCreateButton);

        string cardTitle = cardTitleText.text;
        string cardExplanation = cardExplanationText.text;
        Texture2D tex = imageChosen.sprite.texture;
        
        // カード作成中画面に切り替えてレスポンスを待つ
        imageSelectionPanel.SetActive(false);
        cardGeneratingPanel.SetActive(true);
        var cardData = await _promptFormatter.OrganizePrompt(cardTitle, cardExplanation, tex);
        
        // レスポンスを反映させてカードをデータベースに保存
        cardGeneratingPanel.SetActive(false);
        cardResultPanel.SetActive(true);
        
        await _cardController.OnCardGenerated(cardData, cardTitle, tex);
    }

    public void OnClickBackToTitle()
    {
        
    }
}
