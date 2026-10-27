using UnityEngine;

public class AudioGlobalManager : MonoBehaviour
{
    public static AudioGlobalManager Instance;

    [Header("背景音乐播放器")]
    public AudioSource bgmSource;
    [Header("全局音效播放器")]
    public AudioSource sfxSource;

    [Header("音效素材")]
    public AudioClip ClickSound;
    public AudioClip PuzzleSuccessSound;
    public AudioClip GoldPaySound;
    public AudioClip CutFoodSound;


    void Awake()
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

    #region BGM音量控制
    public void SetBGMVolume(float vol)
    {
        if (bgmSource != null) bgmSource.volume = vol;
    }

    public void SetBGMMute(bool isMute)
    {
        if (bgmSource != null) bgmSource.mute = isMute;
    }
    #endregion

    #region 音效音量控制
    public void SetSFXVolume(float vol)
    {
        if (sfxSource != null) sfxSource.volume = vol;
    }

    public void SetSFXMute(bool isMute)
    {
        if (sfxSource != null) sfxSource.mute = isMute;
    }
    #endregion

    #region 播放音效方法
    public void PlayClick()
    {
        if (ClickSound != null)
            sfxSource.PlayOneShot(ClickSound);
    }
    public void PlayPuzzleSuccess()
    {
        if (PuzzleSuccessSound != null)
            sfxSource.PlayOneShot(PuzzleSuccessSound);
    }
    public void PlayGoldPay()
    {
        if (GoldPaySound != null)
            sfxSource.PlayOneShot(GoldPaySound);
    }
    public void PlayCutFood()
    {
        if (CutFoodSound != null)
            sfxSource.PlayOneShot(CutFoodSound);
    }
    #endregion
}