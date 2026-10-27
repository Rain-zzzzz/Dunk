using UnityEngine;
using UnityEngine.UI;

public class CartItemSlot : MonoBehaviour
{
    public Image iconImg;
    public Text nameText;
    public Text priceText;
    public Text countText;
    public Text singleTotalText;
    public Button btnMinus;
    public Button btnPlus;

    private ShoppingCartManager cartMgr;
    private ItemData targetItem;
    private int unitPrice;
    private int selectCount;

    // 初始化条目数据
    public void Init(ItemData item, int price, int needNum, ShoppingCartManager manager)
    {
        cartMgr = manager;
        targetItem = item;
        unitPrice = price;
        // 默认选购数量=清单需要的数量
        selectCount = needNum;

        iconImg.sprite = item.icon;
        nameText.text = item.itemName;
        priceText.text = $"单价：{unitPrice}金币";
        RefreshCountUI();

        // 绑定加减按钮
        btnMinus.onClick.RemoveAllListeners();
        btnMinus.onClick.AddListener(OnMinus);
        btnPlus.onClick.RemoveAllListeners();
        btnPlus.onClick.AddListener(OnPlus);
    }

    // 减少数量，最低1
    void OnMinus()
    {
        if (selectCount > 1)
        {
            selectCount--;
            RefreshCountUI();
            cartMgr.CalculateTotalPrice();
        }
    }

    // 增加数量，无上限（可自行加限购）
    void OnPlus()
    {
        selectCount++;
        RefreshCountUI();
        cartMgr.CalculateTotalPrice();
    }

    // 刷新当前条目数字与小计
    void RefreshCountUI()
    {
        countText.text = selectCount.ToString();
        singleTotalText.text = $"小计：{unitPrice * selectCount}金币";
    }

    // 对外暴露当前商品与选购数量，结算时读取
    public ItemData GetItem() => targetItem;
    public int GetSelectCount() => selectCount;
    public int GetSinglePrice() => unitPrice;
}