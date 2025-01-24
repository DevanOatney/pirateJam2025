using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100; // Maximum health
    public int currentHealth;

    public delegate void OnHealthChanged(int currentHealth, int maxHealth);
    public event OnHealthChanged onHealthChanged; // Event for UI updates or effects

    void Start()
    {
        currentHealth = maxHealth; // Initialize health
        onHealthChanged?.Invoke(currentHealth, maxHealth); // Notify listeners of initial health
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health doesn't go below 0
        onHealthChanged?.Invoke(currentHealth, maxHealth); // Notify listeners

        if (currentHealth <= 0)
        {
            Die(); // Call death logic if health reaches 0
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health doesn't exceed max
        onHealthChanged?.Invoke(currentHealth, maxHealth); // Notify listeners
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        // Implement death logic here (e.g., respawn, game over, etc.)
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
