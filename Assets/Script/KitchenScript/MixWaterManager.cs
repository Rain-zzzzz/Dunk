using UnityEngine;
using UnityEngine.UI;

public class MixWaterManager : MonoBehaviour
{
    public Slider sliderHoney;
    public Slider sliderSugar;
    public Slider sliderMaltose;
    public Slider sliderVinegar;

    public Text resultText; // 可选，用于实时显示状态

    private float honeyVal, sugarVal, maltoseVal, vinegarVal;

    // 当任何滑块数值改变时调用（挂载到所有Slider的OnValueChanged）
    public void OnSliderValueChanged()
    {
        honeyVal = sliderHoney.value;
        sugarVal = sliderSugar.value;
        maltoseVal = sliderMaltose.value;
        vinegarVal = sliderVinegar.value;

        // 判定规则：蜂蜜:白糖 = 2:1；麦芽糖:大红浙醋 = 1:1
        bool isRatio1Valid = false;
        bool isRatio2Valid = false;

        // 规则1（H:S）
        float total1 = honeyVal + sugarVal;
        if (total1 > 0.01f)
        {
            float ratio1 = honeyVal / total1; // 理论应为 0.666...
            // 允许±10% → 0.6 ~ 0.733
            isRatio1Valid = (ratio1 >= 0.6f && ratio1 <= 0.733f);
        }

        // 规则2（M:V）
        float total2 = maltoseVal + vinegarVal;
        if (total2 > 0.01f)
        {
            float ratio2 = maltoseVal / total2; // 理论应为 0.5
            // 允许±10% → 0.45 ~ 0.55
            isRatio2Valid = (ratio2 >= 0.45f && ratio2 <= 0.55f);
        }

        // 如果两边都为0，或者缺少任意一组，直接不合格
        if (total1 < 0.01f || total2 < 0.01f)
        {
            GameData.isSaucePerfect = false;
            resultText.text = "❌ 缺少原料，请按比例添加";
            return;
        }

        GameData.isSaucePerfect = isRatio1Valid && isRatio2Valid;
        resultText.text = GameData.isSaucePerfect ? "✅ 脆皮水比例完美！" : "⚠️ 比例不对，再调整一下（蜂蜜:白糖=2:1，麦芽糖:大红浙醋=1:1）";
    }
}