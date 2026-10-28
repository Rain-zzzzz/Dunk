using UnityEngine;

public class DraggableItemTag : MonoBehaviour
{
    [Tooltip("物品唯一标识，对应 DropToAnimator 映射表中的 itemName 或 triggerName")]
    public string itemName = "";
}
