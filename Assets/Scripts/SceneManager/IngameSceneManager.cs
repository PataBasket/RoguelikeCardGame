using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameSceneManager : MonoBehaviour
{
    public static IngameSceneManager Instance { get; private set; }
    private Parameter _currentParam;

    public enum InGameState
    {
        Title,
        Lobby,
        EditDeck,
        CardGeneration,
        Map,
        Battle,
        GameClear,
        GameOver,
    }

    private InGameState _currentState = InGameState.Title;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeScene(InGameState state, Parameter param = null)
    {
        _currentState = state;
        _currentParam = param;
        SceneManager.LoadScene(state.ToString());
    }

    public T GetCurrentParameter<T>() where T : Parameter
    {
        return _currentParam as T;
    }
}
