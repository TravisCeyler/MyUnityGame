using UnityEngine;

public class PlayerSpawnStarter : MonoBehaviour
{
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (PlayerPrefs.HasKey("SpawnX"))
        {
            Vector3 spawnPos = new Vector3(
                PlayerPrefs.GetFloat("SpawnX"),
                PlayerPrefs.GetFloat("SpawnY"),
                PlayerPrefs.GetFloat("SpawnZ")
            );

            // Disable controller to move safely
            controller.enabled = false;
            transform.position = spawnPos;
            controller.enabled = true;

            // Clear saved spawn
            PlayerPrefs.DeleteKey("SpawnX");
            PlayerPrefs.DeleteKey("SpawnY");
            PlayerPrefs.DeleteKey("SpawnZ");
        }
    }
}
