using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public abstract int maxHealth { get; }
    public abstract int attackDamage { get; }

    public int currentHealth { get; protected set; }
    

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"[Entity] {gameObject.name} took {amount} damage. (Health: {currentHealth})");

        if (currentHealth <= 0)
        {
            Die(); // THEY FREAKIGN DIED!!!!!!
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log($"[Entity] {gameObject.name} recovered {amount}. (Health: {currentHealth})");
    }

    public void Die()
    {
        Debug.Log($"[Entity] {gameObject.name} has died.");
        OnDeath();
    }

    protected abstract void OnHealthLost();
    protected abstract void OnHealthGained();
    protected abstract void OnDeath();

    
}
