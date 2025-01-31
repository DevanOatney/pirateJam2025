using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    public Transform player; // Reference to the player

    // What projectile to launch
    [SerializeField] GameObject projectile;
    // Where to launch projectile from
    [SerializeField] Transform projectileSpawnPoint;
    [SerializeField] float projectileLaunchForce;
    [SerializeField] float cooldown = 1.5f;

    float launchTimer = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        launchTimer += Time.deltaTime;
        if (launchTimer >= cooldown)
        {
            launchTimer = 0f;

            Vector3 direction1 = new Vector3(1, 0, 0);
            Vector3 direction2 = new Vector3(0, 0, -1);
            Vector3 direction3 = new Vector3(-1, 0, 0);
            Vector3 direction4 = new Vector3(0, 0, 1);
            Vector3 direction5 = new Vector3(1, 0, 1);
            Vector3 direction6 = new Vector3(1, 0, -1);
            Vector3 direction7 = new Vector3(-1, 0, -1);
            Vector3 direction8 = new Vector3(-1, 0, 1);

            BurstAttack(direction1);
            BurstAttack(direction2);
            BurstAttack(direction3);
            BurstAttack(direction4);
            BurstAttack(direction5);
            BurstAttack(direction6);
            BurstAttack(direction7);
            BurstAttack(direction8);
        }
    }

    public virtual void BurstAttack(Vector3 direction)
    {
        // Launch projectile
        GameObject spawnedProjectile = Instantiate(projectile, projectileSpawnPoint.position, quaternion.identity);
        ProjectileBase newProjectile = spawnedProjectile.GetComponent<ProjectileBase>();
        newProjectile.GetComponent<Hitbox>().owner = GetComponent<Entity>();
        newProjectile.transform.rotation = transform.rotation;
        if (newProjectile)
        {
            newProjectile.LaunchProjectile(direction, projectileLaunchForce);
        }
    }
}
