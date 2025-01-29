using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public int damage = 10; // Damage per hit

    private void OnTriggerEnter(Collider other)
    {
        // Check if the hitbox collides with the player
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            // Apply damage to the player
            playerHealth.TakeDamage(damage);
        }
    }
}