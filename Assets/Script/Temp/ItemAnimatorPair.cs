using System;
using UnityEngine;

[Serializable]
public class ItemAnimatorPair
{
    [Tooltip("食材标识，与 DraggableItemTag.itemName 对应；留空 = 单段式：触发物拖入即直接完整播放")]
    public string itemName = "";

    [Tooltip("允许触发完整播放的触发物标识（如「刀」）；留空 = 任意触发物均可")]
    public string triggerName = "";

    [Tooltip("该食材对应的动画名（Animator.Play 用）")]
    public string animName = "";

    [System.NonSerialized]
    public bool played = false;
}
