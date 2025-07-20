using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBattleView : MonoBehaviour
{
    [SerializeField] private Text hpText;
    [SerializeField] private Image hpBar;

    [SerializeField] private Button rockButton;
    [SerializeField] private Button scissorsButton;
    [SerializeField] private Button paperButton;

    [SerializeField] private Text rockPowerText;
    [SerializeField] private Text scissorsPowerText;
    [SerializeField] private Text paperPowerText;

    [SerializeField] private Image cardImage;
    [SerializeField] private Text cardNameText;

    [SerializeField] private Text attackText;

    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Text resultText;
    [SerializeField] private Text scoreText;

    private string rockName;
    private string scissorsName;
    private string paperName;

    public IObservable<Unit> OnRockButtonClick => rockButton.OnClickAsObservable();
    public IObservable<Unit> OnScissorsButtonClick => scissorsButton.OnClickAsObservable();
    public IObservable<Unit> OnPaperButtonClick => paperButton.OnClickAsObservable();

    public void Initialize(Sprite cardImage, string cardName, int rockPower, int scissorsPower, int paperPower, string rockName = "Rock", string scissorsName = "Scissors", string paperName = "Paper")
    {
        // 初期化処理があればここに書く
        resultPanel.SetActive(false);

        // ボタンのテキストを設定
        rockPowerText.text = $"{rockPower}";
        scissorsPowerText.text = $"{scissorsPower}";
        paperPowerText.text = $"{paperPower}";

        // カードの画像と名前を設定
        this.cardImage.sprite = cardImage;
        cardNameText.text = cardName;

        // 各手の名前を設定
        this.rockName = rockName;
        this.scissorsName = scissorsName;
        this.paperName = paperName;

        // 初期HPの表示
        UpdateHPText(100); // 仮の初期値

        Debug.Log($"カード名: {cardName}, グーの攻撃力: {rockPower}, チョキの攻撃力: {scissorsPower}, パーの攻撃力: {paperPower}");

        // 結果パネルの非表示
        resultPanel.SetActive(false);
    }

    public void UpdateHPText(int hp)
    {
        hpText.text = $"{hp}";
        hpBar.fillAmount = hp / 100f; // HPの最大値を100と仮定

        // カード画像を小刻みに振動（ダメージ表現）
        cardImage.rectTransform.DOShakePosition(
            duration: 0.3f,      // 振動の長さ
            strength: new Vector3(30f, 0, 0),  // 横方向の振動
            vibrato: 10,         // 振動回数
            randomness: 90       // ランダム性
        ).SetEase(Ease.OutQuad);
    }

    public void UpdateAttackText(BattleModel.HandType handType)
    {
        Button targetButton = null;
        switch (handType)
        {
            case BattleModel.HandType.Rock:
                attackText.text = $"{rockName}";
                targetButton = rockButton;
                break;
            case BattleModel.HandType.Scissors:
                attackText.text = $"{scissorsName}";
                targetButton = scissorsButton;
                break;
            case BattleModel.HandType.Paper:
                attackText.text = $"{paperName}";
                targetButton = paperButton;
                break;
        }

        if (targetButton != null)
        {
            RectTransform btnTransform = targetButton.GetComponent<RectTransform>();
            // 一度拡大して戻る
            btnTransform.DOComplete(); // 前回アニメが残ってたら止める
            btnTransform.DOKill();     // Killも保険で
            btnTransform.localScale = Vector3.one;
            btnTransform.DOScale(1.5f, 0.15f).SetEase(Ease.OutBack)
                .OnComplete(() => btnTransform.DOScale(1f, 0.15f).SetEase(Ease.InBack));
        }
    }

    public void SuccessAttack()
    {
        // 一瞬だけ大きくなって戻る（攻撃表現）
        cardImage.rectTransform.DOPunchScale(
            punch: new Vector3(1f, 1f, 0), // 拡大率
            duration: 0.25f,                   // 持続時間
            vibrato: 1,                        // 一度だけ
            elasticity: 0.5f                   // 弾力性
        );
    }

    public void UpdateResult(bool result, int score)
    {
        // 結果を表示する処理
        resultText.text = result ? "WIN" : "LOSE";
        scoreText.text = $"Score: {score}";
        resultPanel.SetActive(true);
        Debug.Log($"{resultText} スコア: {score}");
    }

    public void SetButtonsInteractable(bool interactable)
    {
        rockButton.interactable = interactable;
        scissorsButton.interactable = interactable;
        paperButton.interactable = interactable;
    }
}
