using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBattleView : MonoBehaviour
{
    [SerializeField] private Text hpText;

    public void Initialize()
    {
        // 初期化処理があればここに書く
    }

    public void UpdateHPText(int hp)
    {
        hpText.text = $"Enemy HP: {hp}";
    }
}
