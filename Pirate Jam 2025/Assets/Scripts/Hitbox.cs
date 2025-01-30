using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public Entity owner;

    private void OnTriggerEnter(Collider collision)
    {
        // Check if the hitbox collides with an enemy
        Entity target = collision.GetComponent<Entity>();
        if (target != null && target != owner)
        {
            target.TakeDamage(owner.attackDamage);
        }
    }
}
