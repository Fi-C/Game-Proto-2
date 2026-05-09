using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUi : MonoBehaviour
{
    public Inventory Inventory;

    public Transform itemContainer;
    public GameObject itemSlot;

    private void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }


        foreach (ItemData item in Inventory.ItemList)
        {
            GameObject slot = Instantiate(itemSlot, itemContainer);

            TMP_Text text = slot.GetComponentInChildren<TMP_Text>();

            text.text = item.itemName;

            Button button = slot.GetComponent<Button>();  
        }
    }
}
