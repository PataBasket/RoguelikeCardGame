using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TitleButtonAnimation : MonoBehaviour
{
    [SerializeField] private float floatDistance = 20f;  // ふよふよの上下距離（単位: px）
    [SerializeField] private float floatDuration = 0.5f; // 上下にかかる時間

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;

        // Y方向に上下にふよふよ
        transform.DOLocalMoveY(initialPosition.y + floatDistance, floatDuration)
                 .SetEase(Ease.InOutSine)
                 .SetLoops(-1, LoopType.Yoyo);
    }
}
