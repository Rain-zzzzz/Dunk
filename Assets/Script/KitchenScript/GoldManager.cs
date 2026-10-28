using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance;
    [Header("初始金币")]
    public int startGold = 200;
    public int currentGold;

    public delegate void GoldChangeEvent(int newGold);
    public GoldChangeEvent OnGoldChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            PlayerPrefs.DeleteKey("SaveGold");
            PlayerPrefs.Save();

            currentGold = PlayerPrefs.GetInt("SaveGold", startGold);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void AddGold(int num)
    {
        if (num <= 0) return;
        currentGold += num;
        SaveGoldData();
        OnGoldChanged?.Invoke(currentGold);
    }

    public bool DeductGold(int num)
    {
        if (num <= 0 || currentGold < num) return false;
        currentGold -= num;
        SaveGoldData();
        OnGoldChanged?.Invoke(currentGold);
        AudioGlobalManager.Instance?.PlayGoldPay();
        return true;
    }

    public int GetCurrentGold()
    {
        return currentGold;
    }

    void SaveGoldData()
    {
        PlayerPrefs.SetInt("SaveGold", currentGold);
        PlayerPrefs.Save();
    }
}
