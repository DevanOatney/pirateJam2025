public class PlayerHealth : Entity
{
    public override int maxHealth => 100; // Maximum health
    public override int attackDamage => 10;

    public delegate void OnHealthChanged(int currentHealth, int maxHealth);
    public event OnHealthChanged onHealthChanged; // Event for UI updates or effects

    void Start()
    {
        onHealthChanged?.Invoke(currentHealth, maxHealth); // Notify listeners of initial health
    }

    protected override void OnHealthGained()
    {
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    protected override void OnHealthLost()
    {
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    protected override void OnDeath()
    {
        // Implement death logic here (e.g., respawn, game over, etc.)
    }
}
