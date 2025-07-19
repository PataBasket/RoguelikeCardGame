using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewCardData", menuName = "Card Game/Card Data")]
public class CardSequence : ScriptableObject
{
    [System.Serializable]
    public class CardData
    {
        public string cardTitle;
        public Sprite cardImage;
        public int hp;
        public int attackRock;    // グーの攻撃力
        public int attackScissors; // チョキの攻撃力
        public int attackPaper;   // パーの攻撃力
        public string description; // カードの説明
    }

    public List<CardData> notes = new List<CardData>();
}

public class CardSelectManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private CardSelectionCanvasView cardSelectionCanvasView; // Viewへの参照
    [SerializeField] private CardSequence.CardData[] allAvailableCards; // 利用可能な全カードデータ (ScriptableObject)

    // PlayerPrefsで使用するキー
    private const string PLAYER_PREFS_LAST_SELECTED_CARD_TITLE_KEY = "LastSelectedCardTitle";

    // 選択されたカードデータを外部に公開
    public ReadOnlyReactiveProperty<CardSequence.CardData> SelectedCard { get; private set; }
    private ReactiveProperty<CardSequence.CardData> _selectedCard = new ReactiveProperty<CardSequence.CardData>();

    void Awake()
    {
        SelectedCard = _selectedCard.ToReadOnlyReactiveProperty();
    }

    void Start()
    {
        // Viewの初期設定
        cardSelectionCanvasView.PopulateScrollView(allAvailableCards); // 利用可能なカードをスクロールビューに表示

        // Viewからのイベント購読
        cardSelectionCanvasView.OnCardListItemSelected
            .Subscribe(cardData => cardSelectionCanvasView.ShowModalPanel(cardData))
            .AddTo(this);

        cardSelectionCanvasView.OnModalAddToDeckClicked
            .Subscribe(cardData => HandleCardSelection(cardData))
            .AddTo(this);
        
        // PlayerPrefsから前回の選択カードをロードし、表示
        LoadLastSelectedCardFromPlayerPrefs();
        cardSelectionCanvasView.SetSelectedDeckCard(_selectedCard.Value); // デッキの表示を更新
        cardSelectionCanvasView.SetConfirmButtonInteractable(_selectedCard.Value != null); // カードが選択されていればボタン有効化
    }

    /// <summary>
    /// モーダルからカードが選択され、「登録」ボタンが押された際の処理。
    /// </summary>
    /// <param name="cardData">選択されたカードデータ</param>
    private void HandleCardSelection(CardSequence.CardData cardData)
    {
        SoundManager.Instance?.PlaySE(SoundManager.SEData.SETYPE.NormalButton); // カード選択音を再生

        _selectedCard.Value = cardData; // ReactivePropertyの値を更新
        cardSelectionCanvasView.SetSelectedDeckCard(_selectedCard.Value); // 選択されたカードをデッキスロットに表示
        cardSelectionCanvasView.HideModalPanel(); // モーダルを閉じる
        cardSelectionCanvasView.SetConfirmButtonInteractable(true); // バトル開始ボタンを有効にする

        // 最後に選択したカードをPlayerPrefsに保存
        PlayerPrefs.SetString(PLAYER_PREFS_LAST_SELECTED_CARD_TITLE_KEY, _selectedCard.Value.cardTitle);
        PlayerPrefs.Save();
        Debug.Log($"カード '{_selectedCard.Value.cardTitle}' を選択し、PlayerPrefsに保存しました。");
    }

    /// <summary>
    /// PlayerPrefsから最後に選択したカードをロードします。
    /// </summary>
    private void LoadLastSelectedCardFromPlayerPrefs()
    {
        string cardTitle = PlayerPrefs.GetString(PLAYER_PREFS_LAST_SELECTED_CARD_TITLE_KEY, "");
        if (!string.IsNullOrEmpty(cardTitle))
        {
            CardSequence.CardData loadedCard = Array.Find(allAvailableCards, card => card.cardTitle == cardTitle);
            if (loadedCard != null)
            {
                _selectedCard.Value = loadedCard;
                Debug.Log($"PlayerPrefsから最後に選択したカード '{_selectedCard.Value.cardTitle}' をロードしました。");
            }
            else
            {
                Debug.LogWarning($"PlayerPrefsからカード '{cardTitle}' をロードできませんでした。");
            }
        }
    }
}
