using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase instance;
    public ItemData[] allItems;

    private void Awake()
    {
        instance = this;
    }

    public ItemData GetItemByName(string name)
    {
        foreach (var item in allItems)
        {
            if (item.itemName == name)
                return item;
        }
        return null;
    }
}
