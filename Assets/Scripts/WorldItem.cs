using UnityEngine;

public class WorldItem : MonoBehaviour
{
    [Tooltip("Unique ID for this item in the world")]
    public string uniqueID;

    private void OnValidate()
    {
        // Auto-generate a unique ID in the editor
        if (string.IsNullOrEmpty(uniqueID))
            uniqueID = System.Guid.NewGuid().ToString();
    }
    private void Awake()
{
    if (Application.isPlaying && string.IsNullOrEmpty(uniqueID))
    {
        Debug.LogError($"{name} has no uniqueID! Save system will break.");
    }
}
}
