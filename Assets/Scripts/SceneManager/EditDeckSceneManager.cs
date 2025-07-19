using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditDeckSceneManager : MonoBehaviour
{
    [SerializeField]
    private Button lobbyButton;
    [SerializeField]
    private Button lobbyButton2;

    // Start is called before the first frame update
    void Start()
    {
        lobbyButton?.onClick.AddListener(OnLobbyButtonClicked);
        lobbyButton2?.onClick.AddListener(OnAfterResultLobbyButtonClicked);
    }

    private void OnLobbyButtonClicked()
    {
        IngameSceneManager.Instance.ChangeScene(IngameSceneManager.InGameState.Lobby);
    }

    private void OnAfterResultLobbyButtonClicked()
    {
        SoundManager.Instance?.PlayBGM(SoundManager.BGMData.BGMTYPE.Title);
        IngameSceneManager.Instance.ChangeScene(IngameSceneManager.InGameState.Lobby);
    }
}
