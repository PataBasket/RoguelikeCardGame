using System;
using System.Collections;
using System.Collections.Generic;
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

        // 結果パネルの非表示
        resultPanel.SetActive(false);
    }

    public void UpdateHPText(int hp)
    {
        hpText.text = $"{hp}";
        hpBar.fillAmount = hp / 100f; // HPの最大値を100と仮定
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
