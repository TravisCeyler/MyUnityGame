using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleportStarter : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad;
    public Vector3 spawnPosition;
    public bool useSpawnPosition = false;

    [Header("Player Settings")]
    public string playerTag = "Player";

    [Header("Debug")]
    public bool verboseDebug = true;

    private void Start()
    {
        // Ensure teleporter has a trigger collider
        Collider c = GetComponent<Collider>();
        if (c == null)
        {
            Debug.LogWarning($"[{name}] No collider found on teleporter. Add a collider and check 'Is Trigger'.");
        }
        else if (!c.isTrigger)
        {
            Debug.LogWarning($"[{name}] Collider exists but Is Trigger is false. Turn it on for trigger detection.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (verboseDebug) Debug.Log($"[{name}] OnTriggerEnter with: {other.gameObject.name} (tag={other.gameObject.tag})");

        // 1) Tag-based detection (works when the player or a child has "Player" tag)
        if (other.CompareTag(playerTag) || other.transform.root.CompareTag(playerTag))
        {
            if (verboseDebug) Debug.Log($"[{name}] Detected player by tag on '{other.gameObject.name}'. Teleporting...");
            DoTeleport();
            return;
        }

        // 2) CharacterController detection (Starter Assets)
        if (other.GetComponent<CharacterController>() != null || other.GetComponentInParent<CharacterController>() != null)
        {
            if (verboseDebug) Debug.Log($"[{name}] Detected player by CharacterController on '{other.gameObject.name}'. Teleporting...");
            DoTeleport();
            return;
        }

        // 3) Rigidbody-based players (if player uses Rigidbody)
        if (other.attachedRigidbody != null)
        {
            // If you tag the player root as "Player" this will catch it
            if (other.attachedRigidbody.gameObject.CompareTag(playerTag) ||
                other.attachedRigidbody.transform.root.CompareTag(playerTag))
            {
                if (verboseDebug) Debug.Log($"[{name}] Detected player by Rigidbody on '{other.gameObject.name}'. Teleporting...");
                DoTeleport();
                return;
            }
        }

        if (verboseDebug) Debug.Log($"[{name}] Triggered by '{other.gameObject.name}' but not recognized as player.");
    }

    // Fallback for weird setups
    private void OnTriggerStay(Collider other)
    {
        // Small fallback: if OnTriggerEnter failed for some reason, this will catch persistent overlaps.
        OnTriggerEnter(other);
    }

    private void DoTeleport()
    {
        if (useSpawnPosition)
        {
            PlayerPrefs.SetFloat("SpawnX", spawnPosition.x);
            PlayerPrefs.SetFloat("SpawnY", spawnPosition.y);
            PlayerPrefs.SetFloat("SpawnZ", spawnPosition.z);
            PlayerPrefs.Save();
            if (verboseDebug) Debug.Log($"Saved spawn pos {spawnPosition}");
        }

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError($"[{name}] sceneToLoad is empty — can't load scene.");
            return;
        }

        // Load scene
        SceneManager.LoadScene(sceneToLoad);
    }
}
