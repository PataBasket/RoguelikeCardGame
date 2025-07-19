using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSceneManager : MonoBehaviour
{
    [SerializeField]
    private Button robbyButton;

    // Start is called before the first frame update
    void Start()
    {
        robbyButton?.onClick.AddListener(OnRobbyButtonClicked);
    }

    private void OnRobbyButtonClicked()
    {
        SoundManager.Instance?.PlayBGM(SoundManager.BGMData.BGMTYPE.Title);
        IngameSceneManager.Instance.ChangeScene(IngameSceneManager.InGameState.Lobby);
    }
}
