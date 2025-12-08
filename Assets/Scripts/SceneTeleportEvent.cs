using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleportEvent : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad;

    [Header("Spawn Settings (optional)")]
    public bool useSpawnPosition = false;
    public Vector3 spawnPosition;

    [Header("Debug")]
    public bool verboseDebug = true;

    // Called by Keypad UnityEvent
    public void Teleport()
    {
        if (verboseDebug)
            Debug.Log($"[SceneTeleportEvent] Teleport() called. Preparing to load: {sceneToLoad}");

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("[SceneTeleportEvent] Scene name is empty! Add a scene name.");
            return;
        }

        // Save spawn position for next scene
        if (useSpawnPosition)
        {
            PlayerPrefs.SetFloat("SpawnX", spawnPosition.x);
            PlayerPrefs.SetFloat("SpawnY", spawnPosition.y);
            PlayerPrefs.SetFloat("SpawnZ", spawnPosition.z);
            PlayerPrefs.Save();

            if (verboseDebug)
                Debug.Log($"[SceneTeleportEvent] Saved spawn position {spawnPosition}");
        }

        if (verboseDebug)
            Debug.Log($"[SceneTeleportEvent] Loading scene: {sceneToLoad}");

        SceneManager.LoadScene(sceneToLoad);
    }
}
