using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Transform player; // Reference to the player
    public float detectionRange = 10f; // Range within which the enemy detects the player
    public float moveSpeed = 3.5f; // Enemy movement speed
    public float stoppingDistance = 1.5f; // Distance at which the enemy stops moving towards the player

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component
        agent.speed = moveSpeed;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            agent.SetDestination(player.position);

            // Stop moving if within stopping distance
            if (distanceToPlayer <= stoppingDistance)
            {
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
            }
        }
    }
}