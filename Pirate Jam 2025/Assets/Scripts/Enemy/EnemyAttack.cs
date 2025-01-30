using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    public GameObject hitbox; // The enemy's attack hitbox
    public float attackRange = 2f; // Range to detect the player
    public float attackDuration = 0.2f; // Time the hitbox stays active
    public float attackCooldown = 1.5f; // Time between attacks
    public int attackDamage = 10; // Damage per attack

    private bool canAttack = true;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player by tag
        hitbox.SetActive(false); // Ensure hitbox is disabled initially
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If the player is within attack range, attempt an attack
        if (distanceToPlayer <= attackRange && canAttack)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        canAttack = false;

        // Activate the hitbox
        hitbox.SetActive(true);

        // Wait for attack duration
        yield return new WaitForSeconds(attackDuration);

        // Deactivate the hitbox
        hitbox.SetActive(false);

        // Wait for cooldown before attacking again
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}