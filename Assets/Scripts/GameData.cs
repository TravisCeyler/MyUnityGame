using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public PlayerData playerData;
    public List<InventoryItemData> inventoryItems;
}

[Serializable]
public class InventoryItemData
{
    public string itemID;
    public int amount;
}
