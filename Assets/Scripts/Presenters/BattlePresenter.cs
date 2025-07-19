// BattlePresenter.cs
using UnityEngine;
using UniRx;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BattlePresenter : MonoBehaviour
{
    [SerializeField] private PlayerBattleView playerView;
    [SerializeField] private EnemyBattleView enemyView;

    [SerializeField] private CardSelectManager cardSelectManager; // カード選択マネージャーへの参照

    private BattleModel battleModel;

    // 別のシーンからステータスを更新する場合のダミーデータ
    public static BattleModel.CharacterStatus InitialPlayerStatus { get; private set; }
    public static BattleModel.CharacterStatus InitialEnemyStatus { get; private set; }

    // スコア計算のための定数
    private const int BaseScore = 1000; // 基本スコア
    private const int TurnPenalty = 10; // 1ターンあたりの減点
    private const int DrawPenalty = 50; // 引き分け1回あたりの減点
    private const int LossPenalty = 100; // プレイヤーの敗北1回あたりの減点
    private int finalScore = BaseScore;

    void Awake()
    {
        // 初期ステータスが設定されていなければデフォルト値を設定
        if (cardSelectManager.SelectedCard.Value != null)
        {
            // 選択されたカードのデータを基に初期ステータスを設定
            var selectedCard = cardSelectManager.SelectedCard.Value;
            InitialPlayerStatus = new BattleModel.CharacterStatus(
                selectedCard.hp,
                selectedCard.intellect,
                selectedCard.athleticism,
                selectedCard.luck
            );
        }
        else
        {
            // デフォルトの初期ステータスを設定
            InitialPlayerStatus = new BattleModel.CharacterStatus(100, 20, 25, 30); // HP, グー, チョキ, パー
        }

        if (InitialEnemyStatus == null)
        {
            InitialEnemyStatus = new BattleModel.CharacterStatus(80, 15, 20, 25); // HP, グー, チョキ, パー
        }

        battleModel = new BattleModel(InitialPlayerStatus, InitialEnemyStatus);
    }

    void Start()
    {
        // Viewの初期化
        playerView.Initialize();
        enemyView.Initialize();

        // HPの購読
        battleModel.PlayerStatus.HP
            .Subscribe(hp => playerView.UpdateHPText(hp))
            .AddTo(this);

        battleModel.EnemyStatus.HP
            .Subscribe(hp => enemyView.UpdateHPText(hp))
            .AddTo(this);

        // プレイヤーのボタン入力を購読
        playerView.OnRockButtonClick
            .Subscribe(_ => OnPlayerInput(BattleModel.HandType.Rock))
            .AddTo(this);

        playerView.OnScissorsButtonClick
            .Subscribe(_ => OnPlayerInput(BattleModel.HandType.Scissors))
            .AddTo(this);

        playerView.OnPaperButtonClick
            .Subscribe(_ => OnPlayerInput(BattleModel.HandType.Paper))
            .AddTo(this);

        // 初期状態でボタンを有効にする
        playerView.SetButtonsInteractable(true);
    }

    private void OnPlayerInput(BattleModel.HandType playerHand)
    {
        SoundManager.Instance?.PlaySE(SoundManager.SEData.SETYPE.Punch);
        
        // プレイヤー入力があったらボタンを一時的に無効にする
        playerView.SetButtonsInteractable(false);

        // バトル処理を実行
        bool isGameOver = battleModel.ProcessBattle(playerHand);

        // HP更新後、少し遅延させてからボタンを再度有効にする
        Observable.Timer(TimeSpan.FromSeconds(0.5f)) // 0.5秒後に実行
            .Where(_ => !isGameOver) // ゲームオーバーでなければ
            .Subscribe(_ => playerView.SetButtonsInteractable(true))
            .AddTo(this);

        if (isGameOver)
        {
            HandleGameEnd();
        }
    }

    private void HandleGameEnd()
    {
        if (battleModel.PlayerStatus.HP.Value <= 0)
        {
            Debug.Log("ゲームオーバー！");
            // ゲームオーバー時の処理
            playerView.UpdateResult(false, 0); // 仮のスコアを表示
        }
        else if (battleModel.EnemyStatus.HP.Value <= 0)
        {
            Debug.Log("ゲームクリア！");
            CalculateAndDisplayScore(); // ゲームクリア時にスコアを算出
            // ゲームクリア時の処理
            playerView.UpdateResult(true, finalScore); // 仮のスコアを表示
        }
    }

    /// <summary>
    /// ゲームクリア時のスコアを計算し、表示します。
    /// </summary>
    private void CalculateAndDisplayScore()
    {
        // スコア計算
        // ・より短いターン数で勝った方が高得点
        // ・負けやあいこがより少ない方が高得点
        // ・ストレートに全勝で勝った時が最も高得点になるように
        // ・計算には勝つまでのターン数と、各手の出した回数（から派生する勝敗・引き分け数）のみを使用
        finalScore = BaseScore;

        // ターン数による減点
        finalScore -= (battleModel.TurnCount * TurnPenalty);

        // 引き分けによる減点
        finalScore -= (battleModel.DrawCount * DrawPenalty);

        // プレイヤーの敗北（敵の勝利）による減点
        finalScore -= (battleModel.EnemyWinCount * LossPenalty);

        // スコアがマイナスにならないように最小値を0に設定
        finalScore = Mathf.Max(0, finalScore);

        Debug.Log($"--- ゲームクリア！スコア算出 ---");
        Debug.Log($"総ターン数: {battleModel.TurnCount}");
        Debug.Log($"プレイヤー勝利数: {battleModel.PlayerWinCount}");
        Debug.Log($"敵勝利数 (プレイヤー敗北数): {battleModel.EnemyWinCount}");
        Debug.Log($"引き分け数: {battleModel.DrawCount}");
        Debug.Log($"プレイヤーが出した手: グー({battleModel.PlayerHandCounts[BattleModel.HandType.Rock]}) チョキ({battleModel.PlayerHandCounts[BattleModel.HandType.Scissors]}) パー({battleModel.PlayerHandCounts[BattleModel.HandType.Paper]})");
        Debug.Log($"最終スコア: {finalScore}");

        // TODO: 実際のゲームでは、このスコアをGameClearSceneに渡して表示するなどしてください。
        // 例: PlayerPrefs.SetInt("LastGameScore", score);
        // または、staticな変数で次のシーンに渡すなど。
    }

    /// <summary>
    /// 別のシーンからキャラクターの初期ステータスを設定するためのメソッド
    /// 例: シーン遷移時にこのメソッドを呼び出してステータスを渡す
    /// </summary>
    public static void SetInitialCharacterStatus(BattleModel.CharacterStatus playerStatus, BattleModel.CharacterStatus enemyStatus)
    {
        InitialPlayerStatus = playerStatus;
        InitialEnemyStatus = enemyStatus;
    }
}