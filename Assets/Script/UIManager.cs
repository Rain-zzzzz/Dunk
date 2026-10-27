using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    // 原有界面
    [Header("主界面")]
    public GameObject homeUI;
    public GameObject gameUI;
    public GameObject pauseUI;
    public GameObject settingUI;
    public GameObject resultUI;

    // 新增：GameUI 内部子面板
    [Header("游戏内子面板")]
    public GameObject dialogueUI;
    public GameObject puzzleUI;

    [Header("结算页分数文本")]
    public TMP_Text txt_Score;

    private int gameScore;

    // 回到首页
    public void OpenHome()
    {
        homeUI.SetActive(true);
        gameUI.SetActive(false);
        pauseUI.SetActive(false);
        settingUI.SetActive(false);
        resultUI.SetActive(false);

        // 切回主页停止计时
        if (GameTimer.Instance != null)
            GameTimer.Instance.StopTimer();
    }

    // 进入游戏（主页 → GameUI，默认显示对话）
    public void OpenGame()
    {
        homeUI.SetActive(false);
        gameUI.SetActive(true);
        pauseUI.SetActive(false);
        resultUI.SetActive(false);

        dialogueUI.SetActive(false);
        puzzleUI.SetActive(false);
    }
    public void GoToKitchenScene()
    {
        // 加载厨房场景（确保场景名与 Build Settings 中一致）
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level2_Kitchen");
    }
    // 打开暂停
    public void OpenPause()
    {
        pauseUI.SetActive(true);
    }

    // 关闭暂停
    public void ClosePause()
    {
        pauseUI.SetActive(false);
    }

    // 打开设置
    public void OpenSetting()
    {
        settingUI.SetActive(true);
    }

    // 关闭设置
    public void CloseSetting()
    {
        settingUI.SetActive(false);
    }

    // 打开结算界面
    public void OpenResult(int score)
    {
        gameScore = score;
        txt_Score.text = $"本局得分：{gameScore}";
        resultUI.SetActive(true);
    }

    // 关闭结算
    public void CloseResult()
    {
        resultUI.SetActive(false);
    }

    // 退出游戏
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ========== 新增 对话 → 拼图 ==========
    public void GoToPuzzle()
    {
        dialogueUI.SetActive(false);
        puzzleUI.SetActive(true);

        // 初始化拼图+计时
        if (PuzzleManager.Instance != null)
            PuzzleManager.Instance.ShufflePuzzle();
        if (GameTimer.Instance != null)
            GameTimer.Instance.ResetTimer();
    }

    // ========== 新增 拼图 → 返回 GameUI(对话界面) ==========
    // ========== 拼图 → 返回游戏界面 ==========
    public void BackToGamePanel()
    {
        // 1. 关闭拼图界面
        if (puzzleUI != null)
            puzzleUI.SetActive(false);

        // 2. 显示游戏主界面（厨房界面）
        if (gameUI != null)
            gameUI.SetActive(true);

        // 3. 强制重置对话系统（隐藏面板、清空数据）
        DialogueSystem dialogueSys = FindObjectOfType<DialogueSystem>();
        if (dialogueSys != null)
            dialogueSys.ForceEndDialogue();

        // 4. 停止计时器（如果有）
        if (GameTimer.Instance != null)
            GameTimer.Instance.StopTimer();

        // 5. 重置食谱解锁状态（让玩家下次重新挑战）
        GameData.isRecipeUnlocked = false;

        // 6. 可选：隐藏其他可能打开的UI（比如暂停、设置等）
        // pauseUI?.SetActive(false);
        // settingUI?.SetActive(false);

        Debug.Log("已退出拼图，回到游戏界面，对话系统已重置，食谱锁定。");
    }
}