using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CardSelectionCanvasView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject cardLoadPanel; // カード選択パネルのルートGameObject
    [SerializeField] private GameObject cardListItemPrefab; // ScrollViewに配置するカードアイテムのルートプレハブ
    [SerializeField] private Transform scrollViewContent; // ScrollViewのContent Transform

    [SerializeField] private Image selectedDeckCardImage; // 画面上部のデッキスロットのカード画像
    [SerializeField] private Text selectedDeckCardTitleText; // 画面上部のデッキスロットのカードタイトル

    [SerializeField] private GameObject modalPanelObject; // モーダルパネルのルートGameObject
    [SerializeField] private Image modalCardImage;
    [SerializeField] private Text modalCardTitleText;
    [SerializeField] private Text modalCardDescriptionText;
    
    [SerializeField] private Button modalAddToDeckButton; // モーダル内の「登録」ボタン
    [SerializeField] private Button modalCloseButton; // モーダル内の「閉じる」ボタン

    [SerializeField] private GameObject SelectCardCanvas; // バトル開始画面のキャンバス
    [SerializeField] private GameObject BattleCanvas; // バトル開始画面のキャンバス
    [SerializeField] private Button confirmBattleButton; // メインの「バトル開始」ボタン

    [Header("3パラメータのUIオブジェクト")]
    [SerializeField] private Text intellect; // 頭脳の攻撃力
    [SerializeField] private Text intellect_skill; // 頭脳の攻撃名
    [SerializeField] private Text athleticism; // 運動の攻撃力
    [SerializeField] private Text athleticism_skill; // 運動の攻撃名
    [SerializeField] private Text luck; // 運の攻撃力
    [SerializeField] private Text luck_skill; // 運の攻撃名 


    // イベント発行用Subject
    public Subject<CardSelectManager.CardInfo> OnCardListItemSelected = new Subject<CardSelectManager.CardInfo>();
    public Subject<CardSelectManager.CardInfo> OnModalAddToDeckClicked = new Subject<CardSelectManager.CardInfo>();
    public IObservable<Unit> OnModalCloseClicked => modalCloseButton.OnClickAsObservable();
    public IObservable<Unit> OnConfirmBattleClicked => confirmBattleButton.OnClickAsObservable();

    void Awake()
    {
        modalPanelObject.SetActive(false);
        OnModalCloseClicked.Subscribe(_ => HideModalPanel()).AddTo(this);
        modalAddToDeckButton.OnClickAsObservable().Subscribe(_ => OnModalAddToDeckClicked.OnNext(_currentModalCardData)).AddTo(this);
        confirmBattleButton.OnClickAsObservable().Subscribe(_ => BattleStart()).AddTo(this);
        SetConfirmButtonInteractable(false);
        SetSelectedDeckCard(null);
    }

    public void PopulateScrollView(CardSelectManager.CardInfo[] allAvailableCards)
    {
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        foreach (CardSelectManager.CardInfo card in allAvailableCards)
        {
            GameObject itemObject = Instantiate(cardListItemPrefab, scrollViewContent);
            Image cardImage = itemObject.transform.Find("Image").GetComponent<Image>();
            Text cardTitleText = itemObject.transform.Find("Text").GetComponent<Text>();
            Button selectButton = itemObject.GetComponent<Button>();

            if (cardImage != null && cardTitleText != null && selectButton != null)
            {
                cardImage.sprite = card.cardImage;
                cardTitleText.text = card.cardTitle;
                selectButton.OnClickAsObservable()
                    .Subscribe(_ => OnCardListItemSelected.OnNext(card))
                    .AddTo(itemObject);
            }
            else
            {
                Debug.LogError("CardListItemPrefabに必要なUIコンポーネントが見つかりません。", itemObject);
            }
        }
    }

    private CardSelectManager.CardInfo _currentModalCardData;

    public void SetSelectedDeckCard(CardSelectManager.CardInfo cardData)
    {
        if (cardData != null)
        {
            selectedDeckCardImage.sprite = cardData.cardImage;
            selectedDeckCardTitleText.text = cardData.cardTitle;
            selectedDeckCardImage.gameObject.SetActive(true);
        }
        else
        {
            selectedDeckCardImage.sprite = null;
            selectedDeckCardTitleText.text = "";
            selectedDeckCardImage.gameObject.SetActive(false);
        }
    }

    public void ShowLoadPanel(bool show)
    {
        cardLoadPanel.SetActive(show);
    }

    public void ShowModalPanel(CardSelectManager.CardInfo cardData)
    {
        _currentModalCardData = cardData;
        modalCardImage.sprite = cardData.cardImage;
        modalCardTitleText.text = cardData.cardTitle;
        modalCardDescriptionText.text = cardData.description;
        intellect.text = cardData.intellect.ToString();
        intellect_skill.text = cardData.intellect_skill;
        athleticism.text = cardData.athleticism.ToString();
        athleticism_skill.text = cardData.athleticism_skill;
        luck.text = cardData.luck.ToString();
        luck_skill.text = cardData.luck_skill;
        modalPanelObject.SetActive(true);
    }

    public void HideModalPanel()
    {
        SoundManager.Instance?.PlaySE(SoundManager.SEData.SETYPE.NormalButton);
        modalPanelObject.SetActive(false);
        _currentModalCardData = null;
    }

    public void SetConfirmButtonInteractable(bool interactable)
    {
        confirmBattleButton.interactable = interactable;
    }

    public void BattleStart()
    {
        SoundManager.Instance?.PlayBGM(SoundManager.BGMData.BGMTYPE.Battle);

        BattleCanvas.SetActive(true);
        HideModalPanel();
        SelectCardCanvas.SetActive(false);
    }

    void OnDestroy()
    {
        OnCardListItemSelected.Dispose();
        OnModalAddToDeckClicked.Dispose();
    }
}
