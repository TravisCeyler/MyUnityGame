using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }
    private string saveFilePath;

    // Stores all picked-up world item IDs so they stay gone after reload
    public static HashSet<string> collectedItemIDs = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");
    }

    // Data classes
    [System.Serializable]
    private class SaveData
    {
        public Vector3 playerPosition;
        public InventoryItemData[] inventoryItems;
        public string[] collectedItemIDs;
    }

    [System.Serializable]
    private class InventoryItemData
    {
        public string itemName;
        public int amount;
    }

    // Called by pickup code when player collects a world item
    public static void RegisterCollectedItem(string id)
    {
        if (!string.IsNullOrEmpty(id))
            collectedItemIDs.Add(id);
    }

    // Save the entire game state
    public void SaveGame()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Inventory inventory = FindObjectOfType<Inventory>();

        if (player == null || inventory == null)
        {
            Debug.LogWarning("⚠ Player or Inventory not found to save!");
            return;
        }

        SaveData data = new SaveData();
        data.playerPosition = player.transform.position;

        // Save inventory
        data.inventoryItems = new InventoryItemData[inventory.slots.Length];
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            if (inventory.slots[i].item != null)
            {
                data.inventoryItems[i] = new InventoryItemData
                {
                    itemName = inventory.slots[i].item.itemData.itemName,
                    amount = inventory.slots[i].item.amount
                };
            }
        }

        // Save collected world item IDs
        data.collectedItemIDs = new string[collectedItemIDs.Count];
        collectedItemIDs.CopyTo(data.collectedItemIDs);

        // Write to file
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log($"✅ Game saved to {saveFilePath}");
    }

    // Load the saved game state
    public void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("⚠ No save file found!");
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Inventory inventory = FindObjectOfType<Inventory>();

        // Move player safely
        if (player != null)
        {
            var controller = player.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;

            player.transform.position = data.playerPosition;
            Debug.Log($"✅ Player moved to {data.playerPosition}");

            if (controller != null) controller.enabled = true;
        }

        // Restore inventory
        if (inventory != null)
        {
            inventory.ClearInventory();

            if (data.inventoryItems != null)
            {
                for (int i = 0; i < data.inventoryItems.Length; i++)
                {
                    var itemData = data.inventoryItems[i];
                    if (itemData != null && !string.IsNullOrEmpty(itemData.itemName))
                    {
                        ItemData loadedItem = Resources.Load<ItemData>("Items/" + itemData.itemName);
                        if (loadedItem != null)
                            inventory.AddItem(loadedItem, itemData.amount);
                        else
                            Debug.LogWarning($"⚠ Item not found in Resources: {itemData.itemName}");
                    }
                }
            }

            inventory.ForceRefresh();
        }

        // Remove collected world items from the scene
        collectedItemIDs = new HashSet<string>(data.collectedItemIDs ?? new string[0]);
        WorldItem[] worldItems = FindObjectsOfType<WorldItem>();
        foreach (var item in worldItems)
        {
            if (collectedItemIDs.Contains(item.uniqueID))
                Destroy(item.gameObject);
        }

        Debug.Log("✅ Game loaded successfully!");
    }

    // Delete save file
    public void DeleteSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            collectedItemIDs.Clear();
            Debug.Log("🗑 Save file deleted.");
        }
    }
}
