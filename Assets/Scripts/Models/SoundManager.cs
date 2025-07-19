using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

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
        //UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    [SerializeField] AudioSource _bgmAudioSource;
    [SerializeField] AudioSource _seAudioSource;
    [SerializeField] List<BGMData> _bgmSoundDatas;
    [SerializeField] List<SEData> _seSoundDatas;

    BGMData _currentBGM;
    SEData _currentSE;

    [Range(0, 1)] public float masterVolume = 1;
    [Range(0, 1)] public float bgmMasterVolume = 1;
    [Range(0, 1)] public float seMasterVolume = 1;

    private void Start()
    {
        AdjustmentBGM();
        AdjustmentSE();
    }

    private void Update()
    {
        // 音量調整のためのキー入力
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            masterVolume -= 0.1f;
            if (masterVolume < 0) masterVolume = 0;
            AdjustmentBGM();
            AdjustmentSE();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            masterVolume += 0.1f;
            if (masterVolume > 1) masterVolume = 1;
            AdjustmentBGM();
            AdjustmentSE();
        }
    }

    /// <summary>
    /// BGMを流すメソッド
    /// </summary>
    /// <param name="bgm">呼びたいBGM</param>
    public void PlayBGM(BGMData.BGMTYPE bgm)
    {
        BGMData data = _bgmSoundDatas.Find(data => data.bgm == bgm);
        _bgmAudioSource.clip = data.audioClip;
        //_bgmAudioSource.volume = data.volume * bgmMasterVolume * masterVolume;
        _bgmAudioSource.loop = false;
        _bgmAudioSource.Play();
        _currentBGM = data;
        AdjustmentBGM();
        AdjustmentSE();
    }

    public void StopBGM()
    {
        _bgmAudioSource.Stop();
    }


    /// <summary>
    /// 音量調整
    /// </summary>
    public void AdjustmentBGM()
    {
        if (_currentBGM == null) return; // BGMが設定されていない場合は何もしない
        _bgmAudioSource.volume = _currentBGM.volume * bgmMasterVolume * masterVolume;
    }

    public void AdjustmentSE()
    {
        if (_currentSE == null) return; // SEが設定されていない場合は何もしない
        _seAudioSource.volume = _currentSE.volume * seMasterVolume * masterVolume;
    }

    /// <summary>
    /// SEを流すメソッド
    /// </summary>
    /// <param name="se"></param>
    public void PlaySE(SEData.SETYPE se)
    {
        SEData data = _seSoundDatas.Find(data => data.se == se);
        _currentSE = data;
        //_seAudioSource.volume = data.volume * seMasterVolume * masterVolume;
        _seAudioSource.PlayOneShot(data.audioClip);
    }

    [System.Serializable]
    public class BGMData
    {
        public enum BGMTYPE
        {
            //ここの部分がラベルになる
            Title = 0,
            CardCreate = 1,
            Battle = 2
        }

        public BGMTYPE bgm;
        public AudioClip audioClip;
        [Range(0, 1)] public float volume = 1f;
    }

    [System.Serializable]
    public class SEData
    {
        public enum SETYPE
        {
            //ここの部分がラベルになる
            NormalButton = 0,
            CardCreateButton = 1,
            Punch = 2,
        }

        public SETYPE se;
        public AudioClip audioClip;
        [Range(0, 1)] public float volume = 1f;
    }
}
