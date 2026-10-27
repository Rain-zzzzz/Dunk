using UnityEngine;
using UnityEngine.UI;

public class AudioSettingUI : MonoBehaviour
{
    [Header("BGM设置")]
    public Slider slider_BGM;
    public Toggle toggle_BGM;

    [Header("音效设置")]
    public Slider slider_SFX;
    public Toggle toggle_SFX;

    void OnEnable()
    {
        Invoke(nameof(RefreshAudioUI), 0.02f);
    }

    void InitAudioUI()
    {
        if (AudioGlobalManager.Instance == null)
        {
            Debug.LogError("场景缺少AudioGlobalManager全局音频物体！");
            return;
        }
        AudioGlobalManager audioMgr = AudioGlobalManager.Instance;

        if (audioMgr.bgmSource == null)
        {
            Debug.LogError("AudioGlobalManager没有赋值bgmSource！");
            return;
        }
        if (audioMgr.sfxSource == null)
        {
            Debug.LogError("AudioGlobalManager没有赋值sfxSource！");
            return;
        }

        if (slider_BGM == null || toggle_BGM == null || slider_SFX == null || toggle_SFX == null)
        {
            Debug.LogError("音量UI组件没有全部拖拽赋值！检查四个UI引用");
            return;
        }

        //先清空旧事件，防止重复绑定
        slider_BGM.onValueChanged.RemoveAllListeners();
        toggle_BGM.onValueChanged.RemoveAllListeners();
        slider_SFX.onValueChanged.RemoveAllListeners();
        toggle_SFX.onValueChanged.RemoveAllListeners();

        //初始化滑块和开关状态
        slider_BGM.value = audioMgr.bgmSource.volume;
        toggle_BGM.isOn = !audioMgr.bgmSource.mute;
        slider_SFX.value = audioMgr.sfxSource.volume;
        toggle_SFX.isOn = !audioMgr.sfxSource.mute;

        //绑定音量事件
        slider_BGM.onValueChanged.AddListener(audioMgr.SetBGMVolume);
        toggle_BGM.onValueChanged.AddListener(isOn => audioMgr.SetBGMMute(!isOn));
        slider_SFX.onValueChanged.AddListener(audioMgr.SetSFXVolume);
        toggle_SFX.onValueChanged.AddListener(isOn => audioMgr.SetSFXMute(!isOn));
    }

    public void RefreshAudioUI()
    {
        InitAudioUI();
    }
}