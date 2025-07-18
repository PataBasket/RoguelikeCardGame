using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSceneManager : MonoBehaviour
{
    [SerializeField]
    private Button lobbyButton;
    [SerializeField]
    private Button battleButton;

    // Start is called before the first frame update
    void Start()
    {
        lobbyButton?.onClick.AddListener(OnLobbyButtonClicked);
        battleButton?.onClick.AddListener(OnBattleButtonClicked);
    }

    private void OnLobbyButtonClicked()
    {
        IngameSceneManager.Instance.ChangeScene(IngameSceneManager.InGameState.Lobby);
    }

    private void OnBattleButtonClicked()
    {
        IngameSceneManager.Instance.ChangeScene(IngameSceneManager.InGameState.Battle);
    }
}
