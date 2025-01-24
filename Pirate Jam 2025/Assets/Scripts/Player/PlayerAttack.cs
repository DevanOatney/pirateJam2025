using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public KeyCode attackKey; // Key to trigger the attack
    public GameObject hitbox; // Reference to the hitbox GameObject
    public float attackDuration = 0.2f; // Duration for which the hitbox remains active
    public int attackDamage = 10; // Damage dealt by the attack

    private bool isAttacking = false;

    private void Start()
    {
        hitbox.SetActive(false);
    }

    void Update()
    {
        // Check for attack input
        if (Input.GetKeyDown(attackKey) && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private System.Collections.IEnumerator PerformAttack()
    {
        isAttacking = true;

        // Activate the hitbox
        hitbox.SetActive(true);

        // Wait for the attack duration
        yield return new WaitForSeconds(attackDuration);

        // Deactivate the hitbox
        hitbox.SetActive(false);

        isAttacking = false;
    }
}
