using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBattleView : MonoBehaviour
{
    [SerializeField] private Text hpText;
    [SerializeField] private Image hpBar;

    [SerializeField] private Text rockPowerText;
    [SerializeField] private Text scissorsPowerText;
    [SerializeField] private Text paperPowerText;

    [SerializeField] private Image rockButton;
    [SerializeField] private Image scissorsButton;
    [SerializeField] private Image paperButton;

    [SerializeField] private Image cardImage;
    private Text cardNameText;

    [SerializeField] private Text attackText;

    private string rockName = "伸びた鼻つつき";
    private string scissorsName = "ラッキーじまんタイム";
    private string paperName = "学歴マウントトーク";

    public void Initialize()
    {
        // 初期化処理があればここに書く
    }

    public void UpdateHPText(int hp)
    {
        hpText.text = $"{hp}";
        hpBar.fillAmount = hp / 100f; // 0.0f から 1.0f の値を期待

        Debug.Log($"敵のHP更新: {hp}");

        // カード画像を小刻みに振動（ダメージ表現）
        cardImage.rectTransform.DOShakePosition(
            duration: 1.2f,      // 振動の長さ
            strength: new Vector3(30f, 0, 0),  // 横方向の振動
            vibrato: 10,         // 振動回数
            randomness: 90       // ランダム性
        ).SetEase(Ease.OutQuad);
    }

    public void UpdateAttackText(BattleModel.HandType handType)
    {
        Image targetButton = null;
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
            btnTransform.DOScale(1.2f, 0.15f).SetEase(Ease.OutBack)
                .OnComplete(() => btnTransform.DOScale(1f, 0.15f).SetEase(Ease.InBack));
        }
    }

    public void SuccessAttack()
    {
        // 一瞬だけ大きくなって戻る（攻撃表現）
        cardImage.rectTransform.DOPunchScale(
            punch: new Vector3(0.3f, 0.3f, 0), // 拡大率
            duration: 0.25f,                   // 持続時間
            vibrato: 1,                        // 一度だけ
            elasticity: 0.5f                   // 弾力性
        );
    }

    public void UpdateCardInfo(Sprite cardImage, string cardName, int rockPower, int scissorsPower, int paperPower)
    {
        this.cardImage.sprite = cardImage;
        cardNameText.text = cardName;

        rockPowerText.text = $"Rock: {rockPower}";
        scissorsPowerText.text = $"Scissors: {scissorsPower}";
        paperPowerText.text = $"Paper: {paperPower}";
    }
}
