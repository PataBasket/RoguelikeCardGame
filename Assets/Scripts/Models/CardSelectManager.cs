using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CardSelectManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private CardSelectionCanvasView cardSelectionCanvasView; // Viewへの参照
    private CardInfo[] allAvailableCards = new CardInfo[0]; // 利用可能な全カードデータ (ScriptableObject)
    
    // PlayerPrefsで使用するキー
    private const string PLAYER_PREFS_LAST_SELECTED_CARD_TITLE_KEY = "LastSelectedCardTitle";
    
    // 選択されたカードデータを外部に公開
    public ReadOnlyReactiveProperty<CardInfo> SelectedCard { get; private set; }
    private ReactiveProperty<CardInfo> _selectedCard = new ReactiveProperty<CardInfo>();

    void Awake()
    {
        SelectedCard = _selectedCard.ToReadOnlyReactiveProperty();
    }

    async UniTaskVoid Start()
    {
        // 1) Firebaseからロードする
        var records = await CardDatabase.LoadMyCardsAsync();
        
        // 2) CardInfoにマッピング＆画像読み込み
        var infos = new List<CardInfo>();
        foreach (var rec in records)
        {
            var info = new CardInfo {
                cardTitle         = rec.title,
                hp                = rec.intellect,      // サンプル：hp フィールドに intellect を流用
                intellect         = rec.intellect,
                intellect_skill   = rec.intellect_attack,
                athleticism       = rec.athleticism,
                athleticism_skill = rec.athleticism_attack,
                luck              = rec.luck,
                luck_skill        = rec.luck_attack,
                description       = rec.flavor_text
            };

            if (!string.IsNullOrEmpty(rec.imageUrl))
            {
                var uwr = UnityWebRequestTexture.GetTexture(rec.imageUrl);
                await uwr.SendWebRequest();
                if (!uwr.isNetworkError && !uwr.isHttpError)
                {
                    var tex = DownloadHandlerTexture.GetContent(uwr);
                    info.cardImage = Sprite.Create(
                        tex,
                        new Rect(0, 0, tex.width, tex.height),
                        new Vector2(0.5f, 0.5f)
                    );
                }
            }
            
            infos.Add(info);
        }

        allAvailableCards = infos.ToArray();
        
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
    private void HandleCardSelection(CardInfo cardData)
    {
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
            CardInfo loadedCard = Array.Find(allAvailableCards, card => card.cardTitle == cardTitle);
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
    
    [System.Serializable]
    public class CardInfo
    {
        public string cardTitle;
        public Sprite cardImage;
        public int hp;
        public int intellect;    // 頭脳の攻撃力
        public string intellect_skill; // 頭脳の攻撃名
        public int athleticism; // 運動の攻撃力
        public string athleticism_skill;　// 運動の攻撃名
        public int luck;   // 運の攻撃力
        public string luck_skill;　// 運の攻撃名
        public string description; // カードの説明
    }
}
