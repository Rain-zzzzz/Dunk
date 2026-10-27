using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelFlowManager : MonoBehaviour
{
    public GameObject maskTransition; // Mask_Transition 对象（带Animator）
    public Animator maskAnim;
    public string courtyardSceneName = "3D_Courtyard"; // 你的3D场景名字

    // 点击 Btn_SendDry 调用此方法
    public void OnClickSendDry()
    {
        // 检查三项是否全部完成
        if (!GameData.isStuffingDone)
        {
            Debug.Log("填充物还没完成！");
            // 可触发UI提示
            return;
        }
        if (!GameData.isSaucePerfect)
        {
            Debug.Log("脆皮水比例不对！");
            return;
        }
        if (!GameData.isBrushDone)
        {
            Debug.Log("刷皮还没涂均匀！");
            return;
        }

        // 全部完成，触发过渡
        StartCoroutine(TransitionToCourtyard());
    }

    IEnumerator TransitionToCourtyard()
    {
        // 1. 播放遮罩淡入动画（假设Animator有一个 "FadeOut" 触发器）
        maskAnim.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1.5f); // 等待动画播完

        // 2. 异步加载3D场景（保留当前数据）
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(courtyardSceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}