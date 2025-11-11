using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public InventoryItem item; // null if empty
}

public class Inventory : MonoBehaviour
{
    public int maxSlots = 20;
    public InventorySlot[] slots;

    public delegate void OnInventoryChanged();
    public event OnInventoryChanged onInventoryChangedCallback;

    void Awake()
    {
        if (slots == null || slots.Length != maxSlots)
        {
            slots = new InventorySlot[maxSlots];
            for (int i = 0; i < maxSlots; i++)
                slots[i] = new InventorySlot(); // initialize empty slots
        }
    }

    public void AddItem(ItemData newItemData, int amount = 1)
    {
        // Find first empty slot
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].item = new InventoryItem { itemData = newItemData, amount = amount };
                onInventoryChangedCallback?.Invoke();
                return;
            }
            // Optional: stack if same item exists in this slot
            if (slots[i].item.itemData == newItemData)
            {
                slots[i].item.amount += amount;
                onInventoryChangedCallback?.Invoke();
                return;
            }
        }

        Debug.Log("Inventory full!");
    }

    public void DropItem(int slotIndex, Transform dropPoint)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return;

        InventoryItem itemSlot = slots[slotIndex].item;
        if (itemSlot == null || itemSlot.itemData.prefabReference == null)
        {
            Debug.LogWarning("Slot is empty or prefab missing!");
            return;
        }

        GameObject dropped = Instantiate(itemSlot.itemData.prefabReference, dropPoint.position, dropPoint.rotation);
        if (dropped.TryGetComponent(out Rigidbody rb))
            rb.AddForce(dropPoint.forward * 3f, ForceMode.Impulse);

        itemSlot.amount--;
        if (itemSlot.amount <= 0)
            slots[slotIndex].item = null;

        onInventoryChangedCallback?.Invoke();
    }

    public void ForceRefresh()
    {
        onInventoryChangedCallback?.Invoke();
    }
    public void ClearInventory()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].item = null; // clear the item in each slot
        }

        // Update any UI listeners (like hotbar or inventory UI)
        onInventoryChangedCallback?.Invoke();
    }

}
