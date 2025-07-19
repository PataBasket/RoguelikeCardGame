using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbySceneManager : MonoBehaviour
{
    [SerializeField]
    private Button createCardButton;
    [SerializeField]
    private Button editDeckButton;
    [SerializeField]
    private Button mapButton;

    // Start is called before the first frame update
    void Start()
    {
        createCardButton?.onClick.AddListener(OnCreateCardButtonClicked);
        editDeckButton?.onClick.AddListener(OnEditDeckButtonClicked);
        mapButton?.onClick.AddListener(OnMapButtonClicked);
    }

    private void OnCreateCardButtonClicked()
    {
        IngameSceneManager.Instance.ChangeScene(IngameSceneManager.InGameState.CreateCard);
    }

    private void OnEditDeckButtonClicked()
    {
        IngameSceneManager.Instance.ChangeScene(IngameSceneManager.InGameState.EditDeck);
    }

    private void OnMapButtonClicked()
    {
        IngameSceneManager.Instance.ChangeScene(IngameSceneManager.InGameState.Map);
    }
}
