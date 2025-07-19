using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CardListItem : MonoBehaviour
{
    [SerializeField] private Image cardImage;
    [SerializeField] private Text cardTitleText;
    [SerializeField] private Button selectButton;

    private CardSequence.CardData _cardData;
    public CardSequence.CardData CardData => _cardData;

    // このカードが選択されたことを通知するSubject
    public Subject<CardSequence.CardData> OnCardSelected = new Subject<CardSequence.CardData>();

    public void SetCardData(CardSequence.CardData data)
    {
        _cardData = data;
        cardImage.sprite = data.cardImage;
        cardTitleText.text = data.cardTitle;

        selectButton.OnClickAsObservable()
            .Subscribe(_ => OnCardSelected.OnNext(_cardData))
            .AddTo(this);
    }

    void OnDestroy()
    {
        OnCardSelected.Dispose(); // メモリリーク防止
    }
}
