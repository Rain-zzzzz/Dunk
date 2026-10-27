using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogueSystem : MonoBehaviour
{
    [Header("对话UI绑定")]
    public GameObject dialoguePanel;
    public Text speakerNameText;
    public Text dialogueText;
    public Image portraitImage;
    public Button continueButton;

    [Header("打字机速度")]
    public float typeSpeed = 0.05f;
    private bool isTyping;

    [Header("结束行为")]
    public bool shouldOpenPuzzleOnEnd = true;   // 默认 true，因为触发拼图的对话需要它

    [Header("后续操作")]
    public GameObject goKitchenButton;   // 拖入 Btn_GoKitchen

    private List<DialogueLine> currentDialogue;
    private int currentLineIndex;

    void Start()
    {
        // 初始隐藏对话面板
        dialoguePanel.SetActive(false);
        // 绑定继续按钮事件
        continueButton.onClick.AddListener(OnContinueClicked);
    }

    // 外部调用：开启一段对话
    public void StartDialogue(List<DialogueLine> dialogue)
    {
        if (dialogue == null || dialogue.Count == 0) return;

        currentDialogue = dialogue;
        currentLineIndex = 0;
        dialoguePanel.SetActive(true);
        DisplayLine(currentLineIndex);
    }

    // 显示单条对话
    void DisplayLine(int index)
    {
        if (index >= currentDialogue.Count)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = currentDialogue[index];
        speakerNameText.text = line.speakerName;

        // 头像赋值
        if (portraitImage != null && line.speakerPortrait != null)
        {
            portraitImage.sprite = line.speakerPortrait;
        }

        StopAllCoroutines();
        StartCoroutine(TypeText(line.dialogueText));
    }

    // 打字机效果
    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
    }

    // 点击继续按钮
    void OnContinueClicked()
    {
        // 打字中 → 直接显示全部文字
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = currentDialogue[currentLineIndex].dialogueText;
            isTyping = false;
        }
        // 打字结束 → 下一句
        else
        {
            currentLineIndex++;
            DisplayLine(currentLineIndex);
        }
    }

    // 结束对话，隐藏面板，并自动跳转到拼图界面
    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        currentDialogue = null;

        if (shouldOpenPuzzleOnEnd)
        {
            // 打开拼图（原有逻辑）
            if (PuzzleManager.Instance != null)
            {
                if (PuzzleManager.Instance.puzzleRootPanel != null)
                    PuzzleManager.Instance.puzzleRootPanel.SetActive(true);
                PuzzleManager.Instance.ShufflePuzzle();
                GameTimer.Instance.ResetTimer();
            }
        }
        else
        {
            if (goKitchenButton != null)
                goKitchenButton.SetActive(true);
            // 不打开拼图，仅仅结束对话
            Debug.Log("对话结束，不打开拼图。");
            // 可在此激活“前往厨房”按钮（后面会做）
        }

        // 重置标志为默认值，以免影响下一次对话
        shouldOpenPuzzleOnEnd = true;
    }
    public void ForceEndDialogue()
    {
        // 停止所有协程（打字效果）
        StopAllCoroutines();
        // 隐藏对话面板
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        // 清空对话数据，重置索引和打字状态
        currentDialogue = null;
        currentLineIndex = 0;
        isTyping = false;
    }
}