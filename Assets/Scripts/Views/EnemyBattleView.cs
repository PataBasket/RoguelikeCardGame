using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBattleView : MonoBehaviour
{
    [SerializeField] private Text hpText;
    [SerializeField] private Image hpBar;

    [SerializeField] private Text rockPowerText;
    [SerializeField] private Text scissorsPowerText;
    [SerializeField] private Text paperPowerText;

    private Image cardImage;
    private Text cardNameText;

    [SerializeField] private Text attackText;

    private string rockName;
    private string scissorsName;
    private string paperName;

    public void Initialize()
    {
        // 初期化処理があればここに書く
    }

    public void UpdateHPText(int hp)
    {
        hpText.text = $"Enemy HP: {hp}";
    }

    public void UpdateHPBar(int damage)
    {
        hpBar.fillAmount = damage / 100; // 0.0f から 1.0f の値を期待
    }

    public void UpdateAttackText(BattleModel.HandType handType)
    {
        switch (handType)
        {
            case BattleModel.HandType.Rock:
                attackText.text = $"{rockName}";
                break;
            case BattleModel.HandType.Scissors:
                attackText.text = $"{scissorsName}";
                break;
            case BattleModel.HandType.Paper:
                attackText.text = $"{paperName}";
                break;
        }
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
