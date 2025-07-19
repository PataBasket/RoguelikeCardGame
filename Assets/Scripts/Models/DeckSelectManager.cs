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

public class DeckSelectManager : MonoBehaviour
{
    [Header("Available Cards")]
    [SerializeField] private CardSequence.CardData[] allAvailableCards; // Unityエディタで設定する利用可能な全カードデータ
    [SerializeField] private GameObject cardListItemPrefab; // ScrollViewに配置するカードアイテムのプレハブ
    [SerializeField] private Transform scrollViewContent; // ScrollViewのContent Transform

    [Header("Deck Display")]
    [SerializeField] private DeckCardSlot[] deckCardSlots; // 画面上部のデッキスロット (最大3つ)
    private List<CardSequence.CardData> currentDeck = new List<CardSequence.CardData>();
    private const int MAX_DECK_SIZE = 3;

    [Header("Modal Panel")]
    [SerializeField] private DeckSelectView modalPanel;

    [Header("Buttons")]
    [SerializeField] private Button confirmDeckButton;

    // PlayerPrefsで使用するキー
    private const string PLAYER_PREFS_DECK_KEY_PREFIX = "PlayerDeckCard_";
    private const string PLAYER_PREFS_DECK_COUNT_KEY = "PlayerDeckCount";

    void Start()
    {
        // デッキ確認ボタンを初期状態で無効にする（カードが揃うまで）
        confirmDeckButton.interactable = false;

        // 全てのカードアイテムを生成し、スクロールビューに配置
        PopulateScrollView();

        // モーダルパネルの「デッキに追加」ボタンの購読
        modalPanel.OnAddToDeck
            .Subscribe(cardData => AddCardToDeck(cardData))
            .AddTo(this);

        // デッキ確認ボタンの購読
        confirmDeckButton.OnClickAsObservable()
            .Subscribe(_ => ConfirmDeckAndLoadBattle())
            .AddTo(this);

        // PlayerPrefsから前回のデッキをロード
        LoadDeckFromPlayerPrefs();
        UpdateDeckDisplay(); // デッキの表示を更新

        // デッキ枚数に応じて確認ボタンの状態を更新
        CheckDeckValidity();
    }

    private void PopulateScrollView()
    {
        foreach (CardSequence.CardData card in allAvailableCards)
        {
            GameObject itemObject = Instantiate(cardListItemPrefab, scrollViewContent);
            CardListItem item = itemObject.GetComponent<CardListItem>();
            if (item != null)
            {
                item.SetCardData(card);
                // 各カードアイテムが選択されたらモーダルを表示
                item.OnCardSelected
                    .Subscribe(selectedCard => modalPanel.Show(selectedCard))
                    .AddTo(this);
            }
        }
    }

    private void AddCardToDeck(CardSequence.CardData cardData)
    {
        if (currentDeck.Count < MAX_DECK_SIZE)
        {
            // デッキ内に同じカードが既にあるかチェック (必要であれば)
            if (!currentDeck.Contains(cardData))
            {
                currentDeck.Add(cardData);
                UpdateDeckDisplay();
                Debug.Log($"デッキに {cardData.cardTitle} を追加しました。");
            }
            else
            {
                Debug.LogWarning($"{cardData.cardTitle} は既にデッキにあります。");
            }
        }
        else
        {
            Debug.LogWarning("デッキは既に満杯です。(最大3枚)");
        }
        modalPanel.Hide(); // モーダルを閉じる
        CheckDeckValidity(); // デッキ枚数に応じて確認ボタンの状態を更新
    }

    private void UpdateDeckDisplay()
    {
        for (int i = 0; i < MAX_DECK_SIZE; i++)
        {
            if (i < currentDeck.Count)
            {
                deckCardSlots[i].SetCard(currentDeck[i]);
            }
            else
            {
                //deckCardSlots[i].SetCard(null); // カードがないスロットは空にする
            }
        }
    }

    private void CheckDeckValidity()
    {
        // デッキが3枚揃っている場合にボタンを有効にする
        confirmDeckButton.interactable = currentDeck.Count == MAX_DECK_SIZE;
    }

    private void ConfirmDeckAndLoadBattle()
    {
        if (currentDeck.Count != MAX_DECK_SIZE)
        {
            Debug.LogWarning("デッキは3枚必要です。");
            return;
        }

        // PlayerPrefsにデッキ情報を保存
        SaveDeckToPlayerPrefs();

        // CustomPlayerStatusを計算し、BattlePresenterに渡す
        BattleModel.CharacterStatus customPlayerStatus = CalculateCustomPlayerStatus();
        BattlePresenter.SetInitialCharacterStatus(customPlayerStatus, BattlePresenter.InitialEnemyStatus); // 敵のステータスは既存のものを再利用

        Debug.Log("デッキを確定し、バトルシーンへ移行します。");
        IngameSceneManager.Instance.ChangeScene(IngameSceneManager.InGameState.Lobby);
    }

    /// <summary>
    /// デッキ内のカードの合計値からCustomPlayerStatusを計算します。
    /// </summary>
    private BattleModel.CharacterStatus CalculateCustomPlayerStatus()
    {
        int totalHP = 0;
        int totalAttackRock = 0;
        int totalAttackScissors = 0;
        int totalAttackPaper = 0;

        foreach (CardSequence.CardData card in currentDeck)
        {
            totalHP += card.hp;
            totalAttackRock += card.attackRock;
            totalAttackScissors += card.attackScissors;
            totalAttackPaper += card.attackPaper;
        }

        Debug.Log($"Calculated Custom Player Status: HP={totalHP}, Rock={totalAttackRock}, Scissors={totalAttackScissors}, Paper={totalAttackPaper}");
        return new BattleModel.CharacterStatus(totalHP, totalAttackRock, totalAttackScissors, totalAttackPaper);
    }

    /// <summary>
    /// 現在のデッキをPlayerPrefsに保存します。
    /// 各カードのタイトルを保存し、ロード時にCardDataを検索します。
    /// </summary>
    private void SaveDeckToPlayerPrefs()
    {
        PlayerPrefs.SetInt(PLAYER_PREFS_DECK_COUNT_KEY, currentDeck.Count);
        for (int i = 0; i < currentDeck.Count; i++)
        {
            // カードの識別にユニークなID（ここではタイトル）を使用
            PlayerPrefs.SetString(PLAYER_PREFS_DECK_KEY_PREFIX + i, currentDeck[i].cardTitle);
        }
        PlayerPrefs.Save();
        Debug.Log("デッキ情報をPlayerPrefsに保存しました。");
    }

    /// <summary>
    /// PlayerPrefsからデッキ情報をロードします。
    /// </summary>
    private void LoadDeckFromPlayerPrefs()
    {
        currentDeck.Clear();
        int deckCount = PlayerPrefs.GetInt(PLAYER_PREFS_DECK_COUNT_KEY, 0);

        for (int i = 0; i < deckCount; i++)
        {
            string cardTitle = PlayerPrefs.GetString(PLAYER_PREFS_DECK_KEY_PREFIX + i, "");
            if (!string.IsNullOrEmpty(cardTitle))
            {
                // allAvailableCardsの中からタイトルでカードデータを探す
                CardSequence.CardData loadedCard = Array.Find(allAvailableCards, card => card.cardTitle == cardTitle);
                if (loadedCard != null)
                {
                    currentDeck.Add(loadedCard);
                }
                else
                {
                    Debug.LogWarning($"PlayerPrefsからカード '{cardTitle}' をロードできませんでした。データが存在しないか、名前が変更されました。");
                }
            }
        }
        Debug.Log($"PlayerPrefsから{currentDeck.Count}枚のカードをロードしました。");
    }
}
