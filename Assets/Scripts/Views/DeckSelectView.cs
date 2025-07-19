using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DeckSelectView : MonoBehaviour
{
    [SerializeField] private GameObject panelObject; // モーダルパネルのルートGameObject
    [SerializeField] private Image cardImage;
    [SerializeField] private Text cardTitleText;
    [SerializeField] private Text cardDescriptionText;
    [SerializeField] private Text cardStatsText; // HP, 攻撃力などを表示
    [SerializeField] private Button addToDeckButton;
    [SerializeField] private Button closeButton;

    private CardSequence.CardData _currentCardData;

    // デッキに追加ボタンが押されたことを通知するSubject
    public Subject<CardSequence.CardData> OnAddToDeck = new Subject<CardSequence.CardData>();

    void Awake()
    {
        panelObject.SetActive(false); // 初期状態では非表示
        closeButton.OnClickAsObservable().Subscribe(_ => Hide()).AddTo(this);
        addToDeckButton.OnClickAsObservable().Subscribe(_ => OnAddToDeck.OnNext(_currentCardData)).AddTo(this);
    }

    public void Show(CardSequence.CardData cardData)
    {
        _currentCardData = cardData;
        cardImage.sprite = cardData.cardImage;
        cardTitleText.text = cardData.cardTitle;
        cardDescriptionText.text = cardData.description;
        cardStatsText.text = $"HP: {cardData.hp}\nRock Atk: {cardData.attackRock}\nScissors Atk: {cardData.attackScissors}\nPaper Atk: {cardData.attackPaper}";

        panelObject.SetActive(true);
    }

    public void Hide()
    {
        panelObject.SetActive(false);
    }

    void OnDestroy()
    {
        OnAddToDeck.Dispose(); // メモリリーク防止
    }
}
