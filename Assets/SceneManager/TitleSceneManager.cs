using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField]
    private Button startButton;
    [SerializeField]
    private Button configButton;

    // Start is called before the first frame update
    void Start()
    {
        startButton?.onClick.AddListener(OnStartButtonClicked);
        configButton?.onClick.AddListener(OnConfigButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        IngameSceneManager.Instance.ChangeScene(IngameSceneManager.InGameState.Lobby);
        Debug.Log("Start button clicked, changing to Lobby scene.");
    }

    private void OnConfigButtonClicked()
    {
        
    }
}
