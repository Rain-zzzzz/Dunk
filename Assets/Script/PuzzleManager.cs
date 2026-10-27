using UnityEngine;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    [Header("所有拼图碎片（按0~8顺序拖入）")]
    public List<GameObject> allPieces;
    [Header("打乱偏移范围")]
    public float randomRange = 180f;
    [Header("拼图UI根节点")]
    public GameObject puzzleRootPanel;
    [Header("拼图成功后的对话")]
    public List<DialogueLine> successDialogue;   // 在 Inspector 中拖入对话数据
    private bool gameIsOver = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        ShufflePuzzle();
    }

    public void ShufflePuzzle()
    {
        gameIsOver = false;
        foreach (var pieceObj in allPieces)
        {
            PuzzlePiece piece = pieceObj.GetComponent<PuzzlePiece>();
            piece.Unlock();

            RectTransform rect = pieceObj.GetComponent<RectTransform>();
            float randomX = Random.Range(-randomRange, randomRange);
            float randomY = Random.Range(-randomRange, randomRange);
            rect.anchoredPosition = piece.correctPos + new Vector2(randomX, randomY);
        }

        GameTimer.Instance.HideResultText();
    }

    public void CheckWinCondition()
    {
        if (gameIsOver) return;

        int correctCount = 0;
        foreach (var pieceObj in allPieces)
        {
            PuzzlePiece piece = pieceObj.GetComponent<PuzzlePiece>();
            if (piece.isLocked)
            {
                correctCount++;
            }
        }

        if (correctCount >= 9)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        if (gameIsOver) return;
        gameIsOver = true;
        GameTimer.Instance.StopTimer();
        GameTimer.Instance.ShowResult("恭喜！拼图成功！", Color.green);

        // 1. 隐藏拼图界面
        if (puzzleRootPanel != null)
            puzzleRootPanel.SetActive(false);
        else
        {
            foreach (var piece in allPieces)
                piece.SetActive(false);
        }

        // 2. 解锁食谱（但仍可用来判断是否已拼图）
        GameData.isRecipeUnlocked = true;

        // 3. 🆕 触发后续对话（成功后不再打开拼图）
        DialogueSystem dialogueSys = FindObjectOfType<DialogueSystem>();
        if (dialogueSys != null && successDialogue != null && successDialogue.Count > 0)
        {
            dialogueSys.shouldOpenPuzzleOnEnd = false;   // 关键：结束时不打开拼图
            dialogueSys.StartDialogue(successDialogue);
        }
        else
        {
            Debug.LogWarning("没有配置拼图成功后的对话，或找不到 DialogueSystem");
        }

        Debug.Log("拼图完成！食谱已解锁，后续对话已触发。");
    }

    public void LoseGame()
    {
        if (gameIsOver) return;
        gameIsOver = true;
        GameTimer.Instance.StopTimer();
        GameTimer.Instance.ShowResult("时间耗尽，挑战失败", Color.red);

        if (puzzleRootPanel != null)
            puzzleRootPanel.SetActive(false);
        // 不解锁食谱
        Debug.Log("拼图失败，请重新尝试。");
    }
}