using System;
using UnityEngine;

/// <summary>
/// 【食材 + 触发物 → 动画 配对】一条映射记录。
///
/// 逻辑：某个食材拖进触发区后先定格第一帧；只有当「指定名字的触发物」拖入时，
/// 才完整播放该食材对应的动画。
///
/// 字段：
///   - itemName：食材标识（对应 DraggableItemTag.itemName，action=Preview）
///   - triggerName：允许触发完整播放的触发物标识（对应 action=Play 的物品的 itemName）
///   - animName：该食材对应的动画名（Animator.Play 用）
///
/// 使用方式：在 DropToAnimator 的 Inspector 里逐条添加，构成映射表。
/// </summary>
[Serializable]
public class ItemAnimatorPair
{
    [Tooltip("食材标识，与 DraggableItemTag.itemName 对应；留空 = 单段式：触发物拖入即直接完整播放")]
    public string itemName = "";

    [Tooltip("允许触发完整播放的触发物标识（如「刀」）；留空 = 任意触发物均可")]
    public string triggerName = "";

    [Tooltip("该食材对应的动画名（Animator.Play 用）")]
    public string animName = "";

    /// <summary>运行时状态：该条目的动画是否已经完整播放过（用于阻止二次触发）</summary>
    [System.NonSerialized]
    public bool played = false;
}
