using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance;
    [Header("初始金币")]
    public int startGold = 200;
    public int currentGold;

    // 金币变更广播事件，替代直接调用UI，所有界面统一监听
    public delegate void GoldChangeEvent(int newGold);
    public GoldChangeEvent OnGoldChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ====== ⚠️ 临时代码：清除旧金币存档，运行一次后删掉这两行 ======
            PlayerPrefs.DeleteKey("SaveGold");
            PlayerPrefs.Save();
            // ==============================================================

            // 读取本地存档金币（此时 SaveGold 已被删除，返回默认值 startGold=200）
            currentGold = PlayerPrefs.GetInt("SaveGold", startGold);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // 增加金币（剧情/拼图奖励）
    public void AddGold(int num)
    {
        if (num <= 0) return;
        currentGold += num;
        SaveGoldData();
        // 广播通知所有金币UI刷新
        OnGoldChanged?.Invoke(currentGold);
    }

    // 扣除金币（商城采购）
    public bool DeductGold(int num)
    {
        if (num <= 0 || currentGold < num) return false;
        currentGold -= num;
        SaveGoldData();
        // 广播通知所有金币UI刷新
        OnGoldChanged?.Invoke(currentGold);
        // ========= 购买成功，播放金币消耗音效 =========
        AudioGlobalManager.Instance?.PlayGoldPay();
        return true;
    }

    // 获取当前金币数值
    public int GetCurrentGold()
    {
        return currentGold;
    }

    // 本地持久化存档
    void SaveGoldData()
    {
        PlayerPrefs.SetInt("SaveGold", currentGold);
        PlayerPrefs.Save();
    }
}
