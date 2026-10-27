using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 【触发区】"食材 + 触发物 → 动画" 的两段式触发。
///
/// 流程：
///   1. 食材拖入 → 从前往后找「第一个 itemName 匹配且尚未播放过」的条目，
///      播放动画并定格第一帧，同时记住该条目（含其要求的触发物）；
///   2. 触发物拖入 → 用触发物名字校验是否匹配当前定格条目要求的触发物，
///      匹配才完整播放，播放后把该条目标记为已播放。
///
/// 规则：
///   - 已触发（完整播放过）的动画不能二次触发：再次拖入同名食材/触发物会被跳过。
///   - 相同 itemName 对应不同动画时，从前往后排查，取第一个「能触发」的条目执行。
///   - 条目的 itemName 留空 = 单段式：不依赖食材定格，triggerName 对应的物品
///     拖入时直接完整播放该条目动画（同样受「已播放不可二次触发」约束）。
///
/// 使用方式：
///   1. 挂在你想作为"触发区"的物体上（如 Area_Spawn）。
///   2. 填 animMap：每条 = 食材名 + 触发物名 + 动画名。
///   3. Animator 从自身（含子物体）自动查找。
///
/// 注意：本物体需能被 RaycastAll 命中（有 RaycastTarget=true 的 Image，且在 UI 层）。
/// </summary>
public class DropToAnimator : MonoBehaviour
{
    [Header("食材 + 触发物 → 动画 映射表")]
    [Tooltip("每条：食材名 + 触发物名 + 动画名")]
    public List<ItemAnimatorPair> animMap = new List<ItemAnimatorPair>();

    private Animator selfAnimator;
    private ItemAnimatorPair pendingPair = null;  // 当前定格等待播放的映射条目

    private void Awake()
    {
        selfAnimator = GetComponentInChildren<Animator>();
        if (selfAnimator == null)
            Debug.LogWarning($"[DTA] 「{gameObject.name}」自身及子物体上没找到 Animator！");
        else
            Debug.Log($"[DTA] 「{gameObject.name}」找到 Animator，挂在「{selfAnimator.gameObject.name}」上。");
    }

    /// <summary>
    /// 统一入口：物品拖入触发区后，按名字自动判定角色并处理。
    ///   1. 名字命中某条映射的 itemName → 视为食材，定格第一帧；
    ///   2. 否则若名字命中某条映射的 triggerName → 视为触发物：
    ///      - 对应条目的 itemName 为空 → 无需食材，直接完整播放该条目动画；
    ///      - 否则按两段式流程校验当前定格条目后完整播放。
    /// </summary>
    public bool HandleDrop(string itemName)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            Debug.LogWarning("[DTA] HandleDrop 收到空 itemName。");
            return false;
        }

        // 先按食材匹配（从前往后找第一个「能触发」的条目）
        ItemAnimatorPair foodPair = FindFirstFood(itemName);
        if (foodPair != null)
            return PreviewAnim(foodPair);

        // 不是食材，再按触发物匹配
        ItemAnimatorPair triggerPair = FindTriggerPair(itemName);
        if (triggerPair != null)
        {
            // 条目 itemName 为空 = 不依赖食材的两段式流程，触发物拖入即直接播放
            if (string.IsNullOrEmpty(triggerPair.itemName))
                return PlayDirect(triggerPair, itemName);

            return PlayPendingAnim(itemName);
        }

        Debug.LogWarning($"[DTA] 映射表里既没有食材「{itemName}」也没有触发物「{itemName}」！现有条目：{ListMapItems()}");
        return false;
    }

    /// <summary>
    /// 从前往后找第一个 itemName 匹配且尚未播放过的条目。
    /// </summary>
    private ItemAnimatorPair FindFirstFood(string itemName)
    {
        foreach (var pair in animMap)
        {
            if (pair != null && pair.itemName == itemName && !pair.played)
                return pair;
        }
        return null;
    }

    /// <summary>从前往后找第一个 triggerName 匹配且尚未播放过的条目；没有返回 null</summary>
    private ItemAnimatorPair FindTriggerPair(string name)
    {
        foreach (var pair in animMap)
        {
            if (pair != null && pair.triggerName == name && !pair.played)
                return pair;
        }
        return null;
    }

    /// <summary>
    /// 直接完整播放（用于 itemName 为空的条目：不依赖食材定格，触发物拖入即播放）。
    /// 播放后标记已播放，阻止二次触发。
    /// </summary>
    private bool PlayDirect(ItemAnimatorPair pair, string triggerName)
    {
        if (selfAnimator == null || !selfAnimator.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("[DTA] Animator 不可用，无法完整播放。");
            return false;
        }

        selfAnimator.speed = 1f;
        selfAnimator.Play(pair.animName, 0, 0f);
        pair.played = true;
        Debug.Log($"[DTA] 触发物「{triggerName}」命中无食材条目，直接完整播放「{pair.animName}」。");
        return true;
    }

    /// <summary>
    /// 食材拖入：定格对应条目动画的第一帧，并记录该条目（含其要求的触发物）。
    /// </summary>
    public bool PreviewAnim(ItemAnimatorPair pair)
    {
        if (pair == null)
        {
            Debug.LogWarning("[DTA] PreviewAnim 收到空条目。");
            return false;
        }

        pendingPair = pair;

        string req = string.IsNullOrEmpty(pair.triggerName) ? "任意" : pair.triggerName;
        Debug.Log($"[DTA] 食材「{pair.itemName}」就位，动画「{pair.animName}」定格第一帧，要求触发物「{req}」。");
        ShowFirstFrame(pair.animName);
        return true;
    }

    /// <summary>
    /// 触发物拖入：校验名字是否匹配当前定格条目要求的触发物，匹配才完整播放。
    /// 播放后把该条目标记为已播放，阻止二次触发。
    /// </summary>
    public bool PlayPendingAnim(string triggerName)
    {
        if (pendingPair == null)
        {
            Debug.LogWarning($"[DTA] 「{triggerName}」拖入时还没有食材就位，无法播放。");
            return false;
        }

        if (pendingPair.played)
        {
            Debug.LogWarning($"[DTA] 动画「{pendingPair.animName}」已触发过，不能二次触发。");
            pendingPair = null;
            return false;
        }

        // 校验触发物是否匹配（requiredTriggerName 为空 = 任意触发物均可）
        if (!string.IsNullOrEmpty(pendingPair.triggerName) && triggerName != pendingPair.triggerName)
        {
            Debug.LogWarning($"[DTA] 触发物不匹配：动画「{pendingPair.animName}」要求「{pendingPair.triggerName}」，" +
                             $"但拖入的是「{triggerName}」。");
            return false;
        }

        if (selfAnimator == null || !selfAnimator.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("[DTA] Animator 不可用，无法完整播放。");
            return false;
        }

        selfAnimator.speed = 1f;
        selfAnimator.Play(pendingPair.animName, 0, 0f);
        Debug.Log($"[DTA] 触发物「{triggerName}」匹配，完整播放「{pendingPair.animName}」。");

        // 标记已播放，阻止二次触发，并清空待播状态
        pendingPair.played = true;
        pendingPair = null;
        return true;
    }

    /// <summary>定位到动画第一帧并暂停，实现"只显示第一帧"</summary>
    private void ShowFirstFrame(string animName)
    {
        if (selfAnimator == null || !selfAnimator.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("[DTA] Animator 不可用，无法定格。");
            return;
        }

        selfAnimator.Play(animName, 0, 0f); // 跳到第0帧
        selfAnimator.speed = 0f;            // 暂停 → 停在第一帧
    }

    private string ListMapItems()
    {
        string s = "";
        foreach (var p in animMap)
            s += $"[{(p != null ? p.itemName : "null")}{(p != null && p.played ? "(已播)" : "")}] ";
        return string.IsNullOrEmpty(s) ? "(空)" : s;
    }
}
