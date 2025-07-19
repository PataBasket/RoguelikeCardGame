using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBattleView : MonoBehaviour
{
    [SerializeField] private Text hpText;

    [SerializeField] private Button rockButton;
    [SerializeField] private Button scissorsButton;
    [SerializeField] private Button paperButton;

    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Text resultText;
    [SerializeField] private Text scoreText;

    public IObservable<Unit> OnRockButtonClick => rockButton.OnClickAsObservable();
    public IObservable<Unit> OnScissorsButtonClick => scissorsButton.OnClickAsObservable();
    public IObservable<Unit> OnPaperButtonClick => paperButton.OnClickAsObservable();

    public void Initialize()
    {
        // 初期化処理があればここに書く
        resultPanel.SetActive(false);
    }

    public void UpdateHPText(int hp)
    {
        hpText.text = $"Player HP: {hp}";
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
