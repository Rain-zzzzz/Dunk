using UnityEngine;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueSystem dialogueSystem;
    public List<DialogueLine> dialogueToStart;

    // 触发对话的公开方法（给按钮绑定使用）
    public void TriggerDialogue()
    {
        if (dialogueSystem != null)
        {
            dialogueSystem.StartDialogue(dialogueToStart);
        }
    }
}