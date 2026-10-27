using UnityEngine;

[CreateAssetMenu(fileName = "物品", menuName = "背包/物品数据")]
public class ItemData : ScriptableObject
{
    public string itemName;     // 物品名称：苹果/刷子/蜂蜜
    public Sprite icon;     // 物品图标
    public string itemType;     // 分类：Food/Tool/Recipe
    public int stackMax = 99;   // 最大堆叠数量
    // 新增：当前关卡工序需要的数量，直接在每个食材资源里填好
    public int craftNeedNum;
    public int shopPrice; // 商店单价，在Inspector填写
}