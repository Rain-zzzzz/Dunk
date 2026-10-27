using UnityEngine;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public CanvasGroup transitionMask;
    public float fadeSpeed = 0.5f;

    // 淡出：隐藏当前界面，显示目标厨房界面
    public IEnumerator FadeOutToKitchen(GameObject closePanel, GameObject targetKitchen)
    {
        // 黑屏淡入
        while (transitionMask.alpha < 1)
        {
            transitionMask.alpha += Time.deltaTime / fadeSpeed;
            yield return null;
        }
        closePanel.SetActive(false);
        targetKitchen.SetActive(true);
        // 画面淡入，完成场景切换
        while (transitionMask.alpha > 0)
        {
            transitionMask.alpha -= Time.deltaTime / fadeSpeed;
            yield return null;
        }
    }
    // 淡回3D庭院界面
    public IEnumerator FadeBackYard(GameObject closeKitchen, GameObject yardPanel)
    {
        while (transitionMask.alpha < 1)
        {
            transitionMask.alpha += Time.deltaTime / fadeSpeed;
            yield return null;
        }
        closeKitchen.SetActive(false);
        yardPanel.SetActive(true);
        while (transitionMask.alpha > 0)
        {
            transitionMask.alpha -= Time.deltaTime / fadeSpeed;
            yield return null;
        }
    }
}