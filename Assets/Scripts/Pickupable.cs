using UnityEngine;

public class Pickupable : MonoBehaviour
{ 
    public ItemData ItemData;
    public void Pickup(Inventory inventory)
    {
        inventory.AddItem(ItemData);
        Destroy(gameObject);
    }
}
