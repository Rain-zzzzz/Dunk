using System.Collections.Generic;
using UnityEngine;

public class DropToAnimator : MonoBehaviour
{
    [Header("食材 + 触发物 → 动画 映射表")]
    [Tooltip("每条：食材名 + 触发物名 + 动画名")]
    public List<ItemAnimatorPair> animMap = new List<ItemAnimatorPair>();

    private Animator selfAnimator;
    private ItemAnimatorPair pendingPair = null;

    private void Awake()
    {
        selfAnimator = GetComponentInChildren<Animator>();
    }

    public bool HandleDrop(string itemName)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            return false;
        }

        ItemAnimatorPair foodPair = FindFirstFood(itemName);
        if (foodPair != null)
            return PreviewAnim(foodPair);

        ItemAnimatorPair triggerPair = FindTriggerPair(itemName);
        if (triggerPair != null)
        {
            if (string.IsNullOrEmpty(triggerPair.itemName))
                return PlayDirect(triggerPair, itemName);

            return PlayPendingAnim(itemName);
        }

        return false;
    }

    private ItemAnimatorPair FindFirstFood(string itemName)
    {
        foreach (var pair in animMap)
        {
            if (pair != null && pair.itemName == itemName && !pair.played)
                return pair;
        }
        return null;
    }

    private ItemAnimatorPair FindTriggerPair(string name)
    {
        foreach (var pair in animMap)
        {
            if (pair != null && pair.triggerName == name && !pair.played)
                return pair;
        }
        return null;
    }

    private bool PlayDirect(ItemAnimatorPair pair, string triggerName)
    {
        if (selfAnimator == null || !selfAnimator.gameObject.activeInHierarchy)
        {
            return false;
        }

        selfAnimator.speed = 1f;
        selfAnimator.Play(pair.animName, 0, 0f);
        pair.played = true;
        return true;
    }

    public bool PreviewAnim(ItemAnimatorPair pair)
    {
        if (pair == null)
        {
            return false;
        }

        pendingPair = pair;

        string req = string.IsNullOrEmpty(pair.triggerName) ? "任意" : pair.triggerName;
        ShowFirstFrame(pair.animName);
        return true;
    }

    public bool PlayPendingAnim(string triggerName)
    {
        if (pendingPair == null)
        {
            return false;
        }

        if (pendingPair.played)
        {
            pendingPair = null;
            return false;
        }

        if (!string.IsNullOrEmpty(pendingPair.triggerName) && triggerName != pendingPair.triggerName)
        {
            return false;
        }

        if (selfAnimator == null || !selfAnimator.gameObject.activeInHierarchy)
        {
            return false;
        }

        selfAnimator.speed = 1f;
        selfAnimator.Play(pendingPair.animName, 0, 0f);

        pendingPair.played = true;
        pendingPair = null;
        return true;
    }

    private void ShowFirstFrame(string animName)
    {
        if (selfAnimator == null || !selfAnimator.gameObject.activeInHierarchy)
        {
            return;
        }

        selfAnimator.Play(animName, 0, 0f);
        selfAnimator.speed = 0f;
    }

    private string ListMapItems()
    {
        string s = "";
        foreach (var p in animMap)
            s += $"[{(p != null ? p.itemName : "null")}{(p != null && p.played ? "(已播)" : "")}] ";
        return string.IsNullOrEmpty(s) ? "(空)" : s;
    }
}
