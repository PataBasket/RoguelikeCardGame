using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbySceneManager : MonoBehaviour
{
    [SerializeField]
    private Button createCardButton;
    [SerializeField]
    private Button battleButton;

    // Start is called before the first frame update
    void Start()
    {
        createCardButton?.onClick.AddListener(OnCreateCardButtonClicked);
        battleButton?.onClick.AddListener(OnEditDeckButtonClicked);
    }

    private void OnCreateCardButtonClicked()
    {
        IngameSceneManager.Instance.ChangeScene(IngameSceneManager.InGameState.CardGeneration);
    }

    private void OnEditDeckButtonClicked()
    {
        IngameSceneManager.Instance.ChangeScene(IngameSceneManager.InGameState.Battle);
    }
}
