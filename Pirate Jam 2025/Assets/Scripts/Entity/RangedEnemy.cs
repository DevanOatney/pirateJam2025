using UnityEngine;

public class RangedEnemy : Enemy
{
    // What projectile to launch
    [SerializeField] GameObject projectile;
    // Where to launch projectile from
    [SerializeField] Transform projectileSpawnPoint;
    [SerializeField] float projectileLaunchForce;

    protected override void OnAttackStart()
    {
        LaunchProjectile();
    }

    protected override void OnDeath()
    {
        canMove = false;
        canAttack = false;
    }

    protected virtual void LaunchProjectile()
    {
        // Compute launch direction
        Vector3 direction = (target.position - transform.position).normalized;

        // Launch projectile
        GameObject spawnedProjectile = Instantiate(projectile, projectileSpawnPoint.position, Quaternion.identity);
        ProjectileBase newProjectile = spawnedProjectile.GetComponent<ProjectileBase>();

        newProjectile.GetComponent<Hitbox>().owner = this;
        newProjectile.spawnPosition = projectileSpawnPoint.position;
        // newProjectile.maxDistance = attributes.baseAttackRange;
        newProjectile.transform.rotation = transform.rotation;

        if (newProjectile)
        {
            newProjectile.LaunchProjectile(direction, projectileLaunchForce);
        }
    }
}