using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CardSelectionCanvasView : MonoBehaviour
{
     [Header("UI References")]
    [SerializeField] private GameObject cardListItemPrefab; // ScrollViewに配置するカードアイテムのルートプレハブ
    [SerializeField] private Transform scrollViewContent; // ScrollViewのContent Transform

    [SerializeField] private Image selectedDeckCardImage; // 画面上部のデッキスロットのカード画像
    [SerializeField] private Text selectedDeckCardTitleText; // 画面上部のデッキスロットのカードタイトル

    [SerializeField] private GameObject modalPanelObject; // モーダルパネルのルートGameObject
    [SerializeField] private Image modalCardImage;
    [SerializeField] private Text modalCardTitleText;
    [SerializeField] private Text modalCardDescriptionText;
    [SerializeField] private Text modalCardStatsText; // HP, 攻撃力などを表示
    [SerializeField] private Button modalAddToDeckButton; // モーダル内の「登録」ボタン
    [SerializeField] private Button modalCloseButton; // モーダル内の「閉じる」ボタン

    [SerializeField] private GameObject SelectCardCanvas; // バトル開始画面のキャンバス
    [SerializeField] private GameObject BattleCanvas; // バトル開始画面のキャンバス
    [SerializeField] private Button confirmBattleButton; // メインの「バトル開始」ボタン

    // イベント発行用Subject
    public Subject<CardSequence.CardData> OnCardListItemSelected = new Subject<CardSequence.CardData>();
    public Subject<CardSequence.CardData> OnModalAddToDeckClicked = new Subject<CardSequence.CardData>();
    public IObservable<Unit> OnModalCloseClicked => modalCloseButton.OnClickAsObservable();
    public IObservable<Unit> OnConfirmBattleClicked => confirmBattleButton.OnClickAsObservable();

    private CardSequence.CardData _currentModalCardData;

    void Awake()
    {
        modalPanelObject.SetActive(false);
        OnModalCloseClicked.Subscribe(_ => HideModalPanel()).AddTo(this);
        modalAddToDeckButton.OnClickAsObservable().Subscribe(_ => OnModalAddToDeckClicked.OnNext(_currentModalCardData)).AddTo(this);
        confirmBattleButton.OnClickAsObservable().Subscribe(_ => BattleStart()).AddTo(this);
        SetConfirmButtonInteractable(false);
        SetSelectedDeckCard(null);
    }

    public void PopulateScrollView(CardSequence.CardData[] allAvailableCards)
    {
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        foreach (CardSequence.CardData card in allAvailableCards)
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

    public void SetSelectedDeckCard(CardSequence.CardData cardData)
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

    public void ShowModalPanel(CardSequence.CardData cardData)
    {
        _currentModalCardData = cardData;
        modalCardImage.sprite = cardData.cardImage;
        modalCardTitleText.text = cardData.cardTitle;
        modalCardDescriptionText.text = cardData.description;
        modalCardStatsText.text = $"HP: {cardData.hp}\nRock Atk: {cardData.attackRock}\nScissors Atk: {cardData.attackScissors}\nPaper Atk: {cardData.attackPaper}";
        modalPanelObject.SetActive(true);
    }

    public void HideModalPanel()
    {
        modalPanelObject.SetActive(false);
        _currentModalCardData = null;
    }

    public void SetConfirmButtonInteractable(bool interactable)
    {
        confirmBattleButton.interactable = interactable;
    }

    public void BattleStart()
    {
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
