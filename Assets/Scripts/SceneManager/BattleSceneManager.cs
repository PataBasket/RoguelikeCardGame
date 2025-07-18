using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSceneManager : MonoBehaviour
{
    [SerializeField]
    private Button clearButton;
    [SerializeField]
    private Button overButton;

    // Start is called before the first frame update
    void Start()
    {
        clearButton?.onClick.AddListener(OnClearButtonClicked);
        overButton?.onClick.AddListener(OnOverButtonClicked);
    }

    private void OnClearButtonClicked()
    {
        IngameSceneManager.Instance.ChangeScene(IngameSceneManager.InGameState.GameClear);
    }

    private void OnOverButtonClicked()
    {
        IngameSceneManager.Instance.ChangeScene(IngameSceneManager.InGameState.GameOver);
    }
}
