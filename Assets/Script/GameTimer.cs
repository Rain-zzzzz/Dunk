using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    [Header("总限时（秒）")]
    public int totalTime = 60;
    private int currentTime;
    private bool isTiming = false;

    [Header("UI绑定")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI resultText;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        ResetTimer();
    }

    // 每秒倒计时
    void Countdown()
    {
        if (!isTiming) return;

        currentTime--;
        timeText.text = $"剩余时间：{currentTime} 秒";

        if (currentTime <= 0)
        {
            StopTimer();
            PuzzleManager.Instance.LoseGame();
        }
    }

    // 停止计时
    public void StopTimer()
    {
        isTiming = false;
        CancelInvoke("Countdown");
    }

    // 重置并开始计时（给重新开始按钮调用）
    public void ResetTimer()
    {
        StopTimer();
        currentTime = totalTime;
        isTiming = true;
        InvokeRepeating("Countdown", 1f, 1f);
        timeText.text = $"剩余时间：{currentTime} 秒";
        HideResultText();
    }

    // 显示结果文本
    public void ShowResult(string message, Color color)
    {
        resultText.text = message;
        resultText.color = color;
        resultText.gameObject.SetActive(true);
    }

    // 隐藏结果文本
    public void HideResultText()
    {
        resultText.gameObject.SetActive(false);
    }

    // ========== 新增：退出游戏方法 ==========
    public void QuitGame()
    {
#if UNITY_EDITOR
        // Unity 编辑器内：停止播放
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 打包后的程序：直接退出应用
        Application.Quit();
#endif
    }
}