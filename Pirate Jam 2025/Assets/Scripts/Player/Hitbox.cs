using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public int damage = 10; // Damage dealt by this hitbox

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the hitbox collides with an enemy
        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            // Apply damage to the enemy
            enemy.TakeDamage(damage);
        }
    }
}
