using UnityEngine;

public class TableManager : MonoBehaviour
{
    public static TableManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CutApple(DragItem dragItem)
    {
        InventoryManager.Instance.DeductItem(dragItem.GetComponent<ItemData>(), 1);
        Destroy(dragItem.gameObject);
    }

    public void PutCutAppleToPlate(DragItem dragItem)
    {
        InventoryManager.Instance.DeductItem(dragItem.GetComponent<ItemData>(), 1);
        Destroy(dragItem.gameObject);
    }
}