using UnityEngine;

[CreateAssetMenu(fileName = "物品", menuName = "背包/物品数据")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public string itemType;
    public int stackMax = 99;
    public int craftNeedNum;
    public int shopPrice;
}