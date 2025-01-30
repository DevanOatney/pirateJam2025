public class EnemyHealth : Entity
{
    public override int maxHealth => 50;

    public override int attackDamage => 10;

    protected override void OnHealthGained() { }

    protected override void OnHealthLost() { }

    protected override void OnDeath()
    {
        // what happens to this enemy when they die?
    }
}
