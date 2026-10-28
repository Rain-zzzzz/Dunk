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

    public void Init(ItemData item, int price, int needNum, ShoppingCartManager manager)
    {
        cartMgr = manager;
        targetItem = item;
        unitPrice = price;
        selectCount = needNum;

        iconImg.sprite = item.icon;
        nameText.text = item.itemName;
        priceText.text = $"单价：{unitPrice}金币";
        RefreshCountUI();

        btnMinus.onClick.RemoveAllListeners();
        btnMinus.onClick.AddListener(OnMinus);
        btnPlus.onClick.RemoveAllListeners();
        btnPlus.onClick.AddListener(OnPlus);
    }

    void OnMinus()
    {
        if (selectCount > 1)
        {
            selectCount--;
            RefreshCountUI();
            cartMgr.CalculateTotalPrice();
        }
    }

    void OnPlus()
    {
        selectCount++;
        RefreshCountUI();
        cartMgr.CalculateTotalPrice();
    }

    void RefreshCountUI()
    {
        countText.text = selectCount.ToString();
        singleTotalText.text = $"小计：{unitPrice * selectCount}金币";
    }

    public ItemData GetItem() => targetItem;
    public int GetSelectCount() => selectCount;
    public int GetSinglePrice() => unitPrice;
}