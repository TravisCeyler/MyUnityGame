using System;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public ItemData itemData; // Now holds a reference to the ScriptableObject
    public int amount = 1;
}
