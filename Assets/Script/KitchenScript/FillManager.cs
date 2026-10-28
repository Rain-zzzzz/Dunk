using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FillManager : MonoBehaviour
{
    public GameObject recipePopup;
    public Animator itemTableAnim;
    public GameObject choppedPrefab;
    public Transform choppedParent;

    public int targetApple = 2;
    public int targetOnion = 3;
    public int targetOrange = 1;

    private int currentApple = 0;
    private int currentOnion = 0;
    private int currentOrange = 0;
    private int totalFilled = 0;

    public void OnClickRecipeBoard()
    {
        recipePopup.SetActive(!recipePopup.activeSelf);
    }

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
        itemTableAnim.SetTrigger("Chop");
        yield return new WaitForSeconds(0.5f);

        GameObject go = Instantiate(choppedPrefab, choppedParent);

        switch (type)
        {
            case IngredientType.Apple: currentApple++; break;
            case IngredientType.Onion: currentOnion++; break;
            case IngredientType.Orange: currentOrange++; break;
        }
        totalFilled++;

    }

    public void OnClickDuckFill()
    {
        if (currentApple == targetApple && currentOnion == targetOnion && currentOrange == targetOrange)
        {
            GameData.isStuffingDone = true;

            // 填料完成：进度 80%，弹出“步骤五 刷汁”
            PanelManager panelManager = PanelManager.Instance;
            if (panelManager == null) panelManager = FindObjectOfType<PanelManager>(true);
            if (panelManager != null) panelManager.Step5();
        }
        else
        {
        }
    }
}

public enum IngredientType { Apple, Onion, Orange }