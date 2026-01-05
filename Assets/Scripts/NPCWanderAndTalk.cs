using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCWander : MonoBehaviour
{
    [Header("Wandering Settings")]
    public float wanderRadius = 10f;
    public float wanderDelay = 5f;

    private NavMeshAgent agent;
    private float timer;
    private Vector3 startPosition;
    private NPCInteraction npcInteraction;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        npcInteraction = GetComponent<NPCInteraction>();
        startPosition = transform.position;
        timer = wanderDelay;
    }

    void Update()
    {
        // Don’t move while talking
        if (npcInteraction != null && npcInteractionIsTalking())
        {
            agent.isStopped = true;
            return;
        }

        // Resume wandering when not talking
        agent.isStopped = false;

        timer += Time.deltaTime;
        if (timer >= wanderDelay && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            Vector3 newPos = RandomNavSphere(startPosition, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    bool npcInteractionIsTalking()
    {
        // Check if the dialogue is currently open
        return npcInteraction != null && npcInteraction.isActiveAndEnabled &&
               (bool)npcInteraction.GetType().GetField("isDialogueOpen", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(npcInteraction);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * dist + origin;
        NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, dist, layermask);
        return navHit.position;
    }
}
