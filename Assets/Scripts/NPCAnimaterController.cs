using UnityEngine;
using UnityEngine.AI;

public class NPCAnimationController : MonoBehaviour
{
    public Animator animator;
    private NavMeshAgent agent;

    public float moveSpeed = 0.0f; // Set this from your NPC AI script

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Get NPC velocity in local space
        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);

        float moveX = localVelocity.x;
        float moveZ = localVelocity.z;
        float speed = agent.velocity.magnitude;

        // Send to animator
        animator.SetFloat("MoveX", moveX);
        animator.SetFloat("MoveZ", moveZ);
        animator.SetBool("IsMoving", speed > 0.1f);
    }
}
