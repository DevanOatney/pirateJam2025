using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    public GameObject hitbox; // Reference to the hitbox GameObject
    public float attackDuration = 0.2f; // Duration the hitbox remains active
    public float attackCooldown = 0.5f; // Cooldown time between attacks
    public int attackDamage = 10; // Damage dealt by the attack

    private bool isAttacking = false;
    private bool canAttack = true;

    void Update()
    {
        // Check for left mouse click to attack
        if (Input.GetMouseButtonDown(0) && canAttack) // Left Mouse Button (0)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        canAttack = false;

        // Activate the hitbox
        hitbox.SetActive(true);

        // Wait for attack duration
        yield return new WaitForSeconds(attackDuration);

        // Deactivate the hitbox
        hitbox.SetActive(false);

        isAttacking = false;

        // Wait for cooldown before allowing another attack
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}