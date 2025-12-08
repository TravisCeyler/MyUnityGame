using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public InventoryItem item; // null if empty
}

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    public int maxSlots = 20;
    public InventorySlot[] slots;

    public delegate void OnInventoryChanged();
    public event OnInventoryChanged onInventoryChangedCallback;

    void Awake()
    {
        // Singleton persistence
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // destroy duplicates
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (slots == null || slots.Length != maxSlots)
        {
            slots = new InventorySlot[maxSlots];
            for (int i = 0; i < maxSlots; i++)
                slots[i] = new InventorySlot();
        }
    }

    public bool AddItem(ItemData newItemData, int amount = 1)
    {
        // Try stacking first
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null && slots[i].item.itemData == newItemData)
            {
                slots[i].item.amount += amount;
                onInventoryChangedCallback?.Invoke();
                return true;
            }
        }

        // Then find empty slot
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].item = new InventoryItem { itemData = newItemData, amount = amount };
                onInventoryChangedCallback?.Invoke();
                return true;
            }
        }

        Debug.Log("Inventory full!");
        return false;
    }

    public void DropItem(int slotIndex, Transform dropPoint)
{
    if (slotIndex < 0 || slotIndex >= slots.Length)
        return;

    if (dropPoint == null)
    {
        Debug.LogWarning("Drop point is null!");
        return;
    }

    InventoryItem itemSlot = slots[slotIndex].item;
    if (itemSlot == null)
    {
        Debug.LogWarning("Slot is empty!");
        return;
    }

    if (itemSlot.itemData == null || itemSlot.itemData.prefabReference == null)
    {
        Debug.LogWarning("ItemData or prefabReference is null!");
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


    public bool HasItem(ItemData itemData, int amount)
    {
    int total = 0;

    foreach (var slot in slots)
    {
        if (slot.item != null && slot.item.itemData == itemData)
        {
            total += slot.item.amount;
            if (total >= amount)
                return true;
        }
    }

    return false;
    }

    public bool RemoveItem(ItemData itemData, int amount)
{
    int amountToRemove = amount;

    for (int i = 0; i < slots.Length; i++)
    {
        var slot = slots[i].item;

        if (slot != null && slot.itemData == itemData)
        {
            int remove = Mathf.Min(slot.amount, amountToRemove);
            slot.amount -= remove;
            amountToRemove -= remove;

            if (slot.amount <= 0)
                slots[i].item = null;

            if (amountToRemove <= 0)
            {
                onInventoryChangedCallback?.Invoke();
                return true;
            }
        }
    }

    onInventoryChangedCallback?.Invoke();
    return false;
}

}
