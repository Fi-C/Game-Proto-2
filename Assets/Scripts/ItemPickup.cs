using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private Inventory inventory;

    private void Start()
    {
        inventory = GetComponent<Inventory>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Pickupable pickup = other.GetComponent<Pickupable>();

        if (pickup != null)
        {
            pickup.Pickup(inventory);
        }
    }
}