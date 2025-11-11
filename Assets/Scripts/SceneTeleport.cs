using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleportStarter : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad;            // Name of scene to load
    public Vector3 spawnPosition;         // Optional spawn position
    public bool useSpawnPosition = false; // Enable if you want to set a spawn position

    [Header("Player Settings")]
    public string playerTag = "Player";   // Make sure your player is tagged "Player"

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            // Save spawn position for next scene
            if (useSpawnPosition)
            {
                PlayerPrefs.SetFloat("SpawnX", spawnPosition.x);
                PlayerPrefs.SetFloat("SpawnY", spawnPosition.y);
                PlayerPrefs.SetFloat("SpawnZ", spawnPosition.z);
            }

            // Load the next scene
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
