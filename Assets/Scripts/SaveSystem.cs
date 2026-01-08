using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;

    private string savePath;
    private bool loadOnNextScene = false;

    // Track permanently collected items
    private static HashSet<string> collectedItemIDs = new();

    // ===================== DATA CLASSES =====================

    [System.Serializable]
    private class SaveData
    {
        public string sceneName;
        public Vector3 playerPosition;
        public InventoryItemData[] inventory;
        public string[] collectedItems;
        public WorldItemData[] worldItems;
    }

    [System.Serializable]
    private class InventoryItemData
    {
        public string itemName;
        public int amount;
    }

    [System.Serializable]
    private class WorldItemData
    {
        public string id;
        public string itemName;
        public Vector3 position;
        public Quaternion rotation;
    }

    // ===================== UNITY =====================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(Application.persistentDataPath, "savegame.json");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // ===================== PUBLIC API =====================

    public static void RegisterCollectedItem(string id)
    {
        if (!string.IsNullOrEmpty(id))
            collectedItemIDs.Add(id);
    }

    public void SaveGame()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Inventory inventory = FindObjectOfType<Inventory>();

        if (player == null || inventory == null)
        {
            Debug.LogError("❌ Save failed: Player or Inventory missing");
            return;
        }

        SaveData data = new SaveData
        {
            sceneName = SceneManager.GetActiveScene().name,
            playerPosition = player.transform.position,
            collectedItems = new List<string>(collectedItemIDs).ToArray()
        };

        // ---------- INVENTORY ----------
        data.inventory = new InventoryItemData[inventory.slots.Length];
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            if (inventory.slots[i].item != null)
            {
                data.inventory[i] = new InventoryItemData
                {
                    itemName = inventory.slots[i].item.itemData.itemName,
                    amount = inventory.slots[i].item.amount
                };
            }
        }

        // ---------- WORLD ITEMS ----------
        WorldItem[] worldItems = FindObjectsOfType<WorldItem>();
        List<WorldItemData> worldData = new();

        foreach (WorldItem item in worldItems)
        {
            if (collectedItemIDs.Contains(item.uniqueID))
                continue;

            PickupItem pickup = item.GetComponent<PickupItem>();
            if (pickup == null) continue;

            worldData.Add(new WorldItemData
            {
                id = item.uniqueID,
                itemName = pickup.itemData.itemName,
                position = item.transform.position,
                rotation = item.transform.rotation
            });
        }

        data.worldItems = worldData.ToArray();

        File.WriteAllText(savePath, JsonUtility.ToJson(data, true));
        Debug.Log("✅ Game Saved");
    }

    public void LoadGameFromMenu()
    {
        LoadGame();
    }

    // ===================== LOAD LOGIC =====================

    private void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("⚠ No save file found");
            return;
        }

        SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(savePath));
        collectedItemIDs = new HashSet<string>(data.collectedItems ?? new string[0]);

        if (SceneManager.GetActiveScene().name != data.sceneName)
        {
            loadOnNextScene = true;
            SceneManager.LoadScene(data.sceneName);
            return;
        }

        ApplyLoadedData(data);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!loadOnNextScene) return;
        loadOnNextScene = false;

        SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(savePath));
        ApplyLoadedData(data);
    }

    // ===================== APPLY DATA =====================

    private void ApplyLoadedData(SaveData data)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Inventory inventory = FindObjectOfType<Inventory>();

        if (player == null || inventory == null)
        {
            Debug.LogError("❌ Load failed: Player or Inventory missing");
            return;
        }

        // Ensure game is unpaused
        Time.timeScale = 1f;

        // ---------- MOVE PLAYER SAFELY ----------
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        player.transform.position = data.playerPosition;

        if (controller != null) controller.enabled = true;

        // ---------- INVENTORY ----------
        inventory.ClearInventory();
        foreach (var item in data.inventory)
{
    if (item == null) continue;
    if (string.IsNullOrEmpty(item.itemName)) continue;
    if (item.amount <= 0) continue;

    ItemData loaded =
        Resources.Load<ItemData>("Items/" + item.itemName);

    if (loaded != null)
        inventory.AddItem(loaded, item.amount);
    else
        Debug.LogWarning($"⚠ Item not found in Resources: {item.itemName}");
}

        inventory.ForceRefresh();

        // ---------- WORLD ITEMS ----------
        foreach (WorldItem item in FindObjectsOfType<WorldItem>())
            Destroy(item.gameObject);

        foreach (var item in data.worldItems)
        {
            if (collectedItemIDs.Contains(item.id))
                continue;

            ItemData loaded =
                Resources.Load<ItemData>("Items/" + item.itemName);

            if (loaded == null) continue;

            GameObject obj = Instantiate(
                loaded.prefabReference,
                item.position,
                item.rotation
            );

            WorldItem worldItem = obj.GetComponent<WorldItem>();
            if (worldItem != null)
                worldItem.uniqueID = item.id;
        }

        Debug.Log("✅ Game Loaded Successfully");
    }
}
