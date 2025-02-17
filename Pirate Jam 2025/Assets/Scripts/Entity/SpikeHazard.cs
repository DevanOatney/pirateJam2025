using UnityEngine;

public class SpikeHazard : MonoBehaviour
{
    public float attackDamage;
    public float attackCooldown;

    private float attackTimer;

    private void Update()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                attackTimer = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        Entity target = collision.GetComponent<Entity>();

        if (target != null && attackTimer <= 0)
        {
            attackTimer = attackCooldown;
            target.TakeDamage(attackDamage);
        }
    }
}
