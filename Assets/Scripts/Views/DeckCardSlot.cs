using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckCardSlot : MonoBehaviour
{
    [SerializeField] private Image cardImage;
    [SerializeField] private Text cardTitleText;

    public void SetCard(CardSequence.CardData cardData)
    {
        if (cardData != null)
        {
            cardImage.sprite = cardData.cardImage;
            cardTitleText.text = cardData.cardTitle;
            cardImage.gameObject.SetActive(true); // カードがある場合は表示
        }
        else
        {
            // カードが設定されていない場合は非表示にするか、デフォルト画像を表示
            cardImage.sprite = null;
            cardTitleText.text = "EMPTY";
            cardImage.gameObject.SetActive(false); // カードがない場合は非表示
        }
    }
}
