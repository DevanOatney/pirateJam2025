using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 50; // Maximum health of the enemy
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth; // Initialize health
    }

    /// <summary>
    /// Reduces the enemy's health by the given damage amount.
    /// </summary>
    /// <param name="damage">Amount of damage to apply.</param>
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Subtract damage from current health
        Debug.Log($"{gameObject.name} took {damage} damage! Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die(); // Trigger death if health is depleted
        }
    }

    /// <summary>
    /// Handles the enemy's death logic.
    /// </summary>
    private void Die()
    {
        Debug.Log($"{gameObject.name} has died!");

        // You can add additional death logic here, such as playing a death animation
        // and/or dropping loot.

        Destroy(gameObject); // Remove the enemy from the scene
    }

    /// <summary>
    /// Heals the enemy by the given amount.
    /// </summary>
    /// <param name="amount">Amount of health to restore.</param>
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health doesn't exceed max
        Debug.Log($"{gameObject.name} healed for {amount}. Current health: {currentHealth}");
    }
}
