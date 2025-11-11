using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NPCPushable : MonoBehaviour
{
    [Header("Push Settings")]
    public float pushForce = 2f;  // How strong the push feels

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // NavMeshAgent still moves it
    }

    // Called when player controller bumps this collider
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Make sure the collider belongs to the player
        if (hit.controller != null && hit.gameObject.CompareTag("Player"))
        {
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            Vector3 newPos = transform.position + pushDir * pushForce * Time.deltaTime;

            // Move NPC slightly away from player
            rb.MovePosition(newPos);
        }
    }
}
