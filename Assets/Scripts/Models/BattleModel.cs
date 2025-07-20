// BattleModel.cs
using UnityEngine;
using UniRx;
using System;
using System.Collections.Generic; // Dictionaryを使うために追加

public interface IBattleModel
{
    IObservable<Unit> PlayerSuccess { get; }
    IObservable<Unit> EnemySuccess { get; }
}
public class BattleModel: IBattleModel
{
    public IObservable<Unit> PlayerSuccess => playerSuccessSubject;
    public IObservable<Unit> EnemySuccess => enemySuccessSubject;

    private Subject<Unit> playerSuccessSubject = new Subject<Unit>();
    private Subject<Unit> enemySuccessSubject = new Subject<Unit>();

    // 各キャラクターのステータス定義
    public class CharacterStatus
    {
        public ReactiveProperty<int> HP { get; private set; }
        public int AttackRock { get; private set; } // グーの攻撃力
        public int AttackScissors { get; private set; } // チョキの攻撃力
        public int AttackPaper { get; private set; } // パーの攻撃力

        public CharacterStatus(int initialHP, int attackRock, int attackScissors, int attackPaper)
        {
            HP = new ReactiveProperty<int>(initialHP);
            AttackRock = attackRock;
            AttackScissors = attackScissors;
            AttackPaper = attackPaper;
        }

        public void TakeDamage(int damage)
        {
            HP.Value = Mathf.Max(0, HP.Value - damage);
            Debug.Log($"ダメージ！ 現在HP: {HP.Value}");
        }
    }

    public CharacterStatus PlayerStatus { get; private set; }
    public CharacterStatus EnemyStatus { get; private set; }

    public enum HandType { Rock, Scissors, Paper } // グー、チョキ、パー

    // スコア計算用に追加
    public int TurnCount { get; private set; } = 0;
    public int PlayerWinCount { get; private set; } = 0;
    public int EnemyWinCount { get; private set; } = 0; // プレイヤーの敗北数
    public int DrawCount { get; private set; } = 0;

    // 各手の出した回数を記録
    public Dictionary<HandType, int> PlayerHandCounts { get; private set; }

    public ReactiveProperty<HandType> enemyHand = new ReactiveProperty<HandType>(HandType.Rock); // 敵の手をReactivePropertyで管理
    public ReactiveProperty<HandType> playerViewHand = new ReactiveProperty<HandType>(HandType.Rock); // 敵の手をReactivePropertyで管理



    public BattleModel(CharacterStatus playerStatus, CharacterStatus enemyStatus)
    {
        PlayerStatus = playerStatus;
        EnemyStatus = enemyStatus;
        PlayerHandCounts = new Dictionary<HandType, int>
        {
            { HandType.Rock, 0 },
            { HandType.Scissors, 0 },
            { HandType.Paper, 0 }
        };
    }

    /// <summary>
    /// バトルロジックを実行し、結果を返します。
    /// </summary>
    /// <param name="playerHand">プレイヤーが出した手</param>
    /// <returns>ゲームが終了したかどうか</returns>
    public bool ProcessBattle(HandType playerHand)
    {
        TurnCount++; // ターン数をインクリメント
        PlayerHandCounts[playerHand]++; // プレイヤーが出した手の回数を記録

        enemyHand.Value = GetRandomEnemyHand();
        playerViewHand.Value = playerHand; // プレイヤーの手を更新
        Debug.Log($"プレイヤーの手: {playerHand}, 敵の手: {enemyHand}");


        int playerAttack = GetAttackPower(PlayerStatus, playerHand);
        int enemyAttack = GetAttackPower(EnemyStatus, enemyHand.Value);

        // 勝敗判定
        if (playerHand == enemyHand.Value)
        {
            Debug.Log("引き分け！");
            DrawCount++; // 引き分け数をインクリメント
        }
        else if ((playerHand == HandType.Rock && enemyHand.Value == HandType.Scissors) ||
                 (playerHand == HandType.Scissors && enemyHand.Value == HandType.Paper) ||
                 (playerHand == HandType.Paper && enemyHand.Value == HandType.Rock))
        {
            // プレイヤーの勝ち
            Debug.Log("プレイヤーの勝ち！");
            PlayerWinCount++; // プレイヤーの勝利数をインクリメント
            EnemyStatus.TakeDamage(playerAttack);
            playerSuccessSubject.OnNext(Unit.Default); // プレイヤーの成功を通知
        }
        else
        {
            // プレイヤーの負け (敵の勝ち)
            Debug.Log("敵の勝ち！");
            EnemyWinCount++; // 敵の勝利数（プレイヤーの敗北数）をインクリメント
            PlayerStatus.TakeDamage(enemyAttack);
            enemySuccessSubject.OnNext(Unit.Default); // 敵の成功を通知
        }

        return PlayerStatus.HP.Value <= 0 || EnemyStatus.HP.Value <= 0;
    }

    private HandType GetRandomEnemyHand()
    {
        return (HandType)UnityEngine.Random.Range(0, 3);
    }

    private int GetAttackPower(CharacterStatus status, HandType hand)
    {
        switch (hand)
        {
            case HandType.Rock: return status.AttackRock;
            case HandType.Scissors: return status.AttackScissors;
            case HandType.Paper: return status.AttackPaper;
            default: return 0;
        }
    }
}