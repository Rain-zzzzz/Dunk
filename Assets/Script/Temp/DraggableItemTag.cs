using UnityEngine;

/// <summary>
/// 【物品标识】挂在每个可拖拽的 ItemSlotPrefab 上，标记"我是哪个物品"。
///
/// 与 DropToAnimator 配合：触发区松手时读取 itemName，按名字自动判定角色：
///   - 名字命中映射表里的 itemName → 视为「食材」，定格对应动画第一帧；
///   - 名字命中某条映射的 triggerName → 视为「触发物」，尝试完整播放已定格动画。
///
/// 使用方式：
///   1. 在每个 ItemSlotPrefab 根物体（或图标物体）上挂本脚本。
///   2. 填唯一的 itemName（如"苹果"、"刀"），要和 DropToAnimator 映射表里的名字一致。
/// </summary>
public class DraggableItemTag : MonoBehaviour
{
    [Tooltip("物品唯一标识，对应 DropToAnimator 映射表中的 itemName 或 triggerName")]
    public string itemName = "";
}
