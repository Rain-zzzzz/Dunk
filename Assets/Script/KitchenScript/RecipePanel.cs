using UnityEngine;
using UnityEngine.UI;
public class RecipePanel : MonoBehaviour
{
    public GameObject recipePanel;
    public Text recipeText;
    public string duckRecipe =
    "【北京烤鸭制作食谱】\n" +
    "步骤1：切分原材料：苹果、洋葱、橙子切块\n" +
    "步骤2：调配两份配料\n" +
    "▪A液：蜂蜜 : 白糖 = 2 : 1\n" +
    "▪B液：麦芽糖 : 大红浙醋 = 1 : 1\n" +
    "步骤3：将切好果块塞入鸭腹\n" +
    "步骤4：鸭子表皮刷第一层脆皮水\n" +
    "步骤5：第一次风干\n" +
    "步骤6：再刷一遍脆皮水\n" +
    "步骤7：第二次风干\n" +
    "步骤8：放入烤箱烘烤\n" +
    "⚠️时间太长烤焦，时间太短没熟透";
    //打开食谱
    public void OpenRecipe()
    {
        recipePanel.SetActive(true);
        recipeText.text = duckRecipe;
    }
    //关闭食谱
    public void CloseRecipe()
    {
        recipePanel.SetActive(false);
    }
}