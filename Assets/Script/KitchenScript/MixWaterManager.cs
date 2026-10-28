using UnityEngine;
using UnityEngine.UI;

public class MixWaterManager : MonoBehaviour
{
    public Slider sliderHoney;
    public Slider sliderSugar;
    public Slider sliderMaltose;
    public Slider sliderVinegar;

    public Text resultText;

    private float honeyVal, sugarVal, maltoseVal, vinegarVal;

    public void OnSliderValueChanged()
    {
        honeyVal = sliderHoney.value;
        sugarVal = sliderSugar.value;
        maltoseVal = sliderMaltose.value;
        vinegarVal = sliderVinegar.value;

        bool isRatio1Valid = false;
        bool isRatio2Valid = false;

        float total1 = honeyVal + sugarVal;
        if (total1 > 0.01f)
        {
            float ratio1 = honeyVal / total1;
            isRatio1Valid = (ratio1 >= 0.6f && ratio1 <= 0.733f);
        }

        float total2 = maltoseVal + vinegarVal;
        if (total2 > 0.01f)
        {
            float ratio2 = maltoseVal / total2;
            isRatio2Valid = (ratio2 >= 0.45f && ratio2 <= 0.55f);
        }

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