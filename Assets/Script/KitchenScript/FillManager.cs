using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FillManager : MonoBehaviour
{
    public GameObject recipePopup; // 拖拽 Popup_Recipe
    public Animator itemTableAnim; // 食材台的Animator（触发切菜动画）
    public GameObject choppedPrefab; // 切好的配料预制体（比如一个圆形小图标）
    public Transform choppedParent;  // 生成切好配料的父物体（如Grid布局）

    public int targetApple = 2;
    public int targetOnion = 3;
    public int targetOrange = 1;

    private int currentApple = 0;
    private int currentOnion = 0;
    private int currentOrange = 0;
    private int totalFilled = 0;

    // 点击食谱看板
    public void OnClickRecipeBoard()
    {
        recipePopup.SetActive(!recipePopup.activeSelf); // 切换弹窗
    }

    // 点击食材台上的苹果/洋葱/橙子按钮（分别绑定这三个方法）
    public void AddApple()
    {
        StartCoroutine(ChopAndAdd(IngredientType.Apple));
    }
    public void AddOnion()
    {
        StartCoroutine(ChopAndAdd(IngredientType.Onion));
    }
    public void AddOrange()
    {
        StartCoroutine(ChopAndAdd(IngredientType.Orange));
    }

    private IEnumerator ChopAndAdd(IngredientType type)
    {
        // 1. 播放切菜动画
        itemTableAnim.SetTrigger("Chop");
        yield return new WaitForSeconds(0.5f); // 等待动画播放

        // 2. 生成切好的配料图标
        GameObject go = Instantiate(choppedPrefab, choppedParent);

        // 3. 记录计数
        switch (type)
        {
            case IngredientType.Apple: currentApple++; break;
            case IngredientType.Onion: currentOnion++; break;
            case IngredientType.Orange: currentOrange++; break;
        }
        totalFilled++;

        // 4. 更新UI显示（如果你有Text显示数量，可以在这里更新）
    }

    // 点击鸭子（Duck_Fill）触发填充判定
    public void OnClickDuckFill()
    {
        if (currentApple == targetApple && currentOnion == targetOnion && currentOrange == targetOrange)
        {
            GameData.isStuffingDone = true;
            Debug.Log("填充完美！比例正确！");
            // 可以播放一个填充完成特效
        }
        else
        {
            Debug.Log($"填充比例不对！当前 苹果:{currentApple}/{targetApple}, 洋葱:{currentOnion}/{targetOnion}, 橙子:{currentOrange}/{targetOrange}");
            // 提示玩家错误（可弹窗）
        }
    }
}

public enum IngredientType { Apple, Onion, Orange }