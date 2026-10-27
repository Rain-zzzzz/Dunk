using UnityEngine;

public static class GameData
{
    // --- 关卡2 厨房相关 ---
    public static bool isRecipeUnlocked = false;   // 食谱是否已解锁（拼图完成）
    public static bool isStuffingDone = false;     // 填充物是否完成
    public static bool isSaucePerfect = false;     // 脆皮水是否合格
    public static bool isBrushDone = false;        // 刷皮是否均匀

    // --- 全局通用（可选）---
    public static bool hasDuck = true;             // 默认有鸭子（跳过关卡1）
    public static bool isDuckCleaned = true;       // 默认已清洗

    // 重置关卡2数据（用于重新开始）
    public static void ResetLevel2Data()
    {
        isRecipeUnlocked = false;
        isStuffingDone = false;
        isSaucePerfect = false;
        isBrushDone = false;
    }
}