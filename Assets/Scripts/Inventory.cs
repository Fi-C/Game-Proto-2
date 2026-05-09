using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public ItemData selectedItem;
    public List<ItemData> ItemList = new List<ItemData>();
    public InventoryUi inventoryUi;

    public void AddItem(ItemData item)
    {
        ItemList.Add(item);

        Debug.Log(item.itemName + " added!");
        inventoryUi.RefreshUI();
    }
}