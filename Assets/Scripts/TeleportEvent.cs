using UnityEngine;

public class TeleportEvent : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform teleportTarget;          // Place your empty GameObject here
    public bool alignToGround = true;         // Snap to ground using raycast
    public float groundRayDistance = 5f;      // How far down to check for ground
    public LayerMask groundMask = ~0;         // Layers considered as ground

    [Header("Collision Trigger")]
    public bool useCollision = true;          // Enable teleport on collision
    public string playerTag = "Player";       // Tag to detect player

    public void TeleportPlayer(Transform player)
    {
        if (teleportTarget == null)
        {
            Debug.LogWarning("TeleportEvent: No teleport target set!");
            return;
        }

        Vector3 targetPos = teleportTarget.position;
        Quaternion targetRot = teleportTarget.rotation;

        // Optional ground alignment
        if (alignToGround)
        {
            if (Physics.Raycast(teleportTarget.position + Vector3.up, Vector3.down,
                out RaycastHit hit, groundRayDistance, groundMask))
            {
                targetPos = hit.point;
            }
        }

        // Move player using CharacterController if it exists
        var controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            player.SetPositionAndRotation(targetPos, targetRot);
            controller.enabled = true;
        }
        else
        {
            player.SetPositionAndRotation(targetPos, targetRot);
        }

        Debug.Log("Player teleported to " + targetPos);
    }

    // Detect collisions
    private void OnTriggerEnter(Collider other)
    {
        if (!useCollision) return;

        if (other.CompareTag(playerTag))
        {
            TeleportPlayer(other.transform);
        }
    }
}
