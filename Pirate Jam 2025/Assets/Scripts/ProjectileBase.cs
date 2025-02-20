using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    #region Properties

    Rigidbody rb;

    [SerializeField] public float maxTime = 5f;
    [SerializeField] public float maxDistance;

    public Vector3 spawnPosition;

    #endregion Properties

    void Awake()
    {
        ProjectileManager.Instance.activeProjectiles.Add(this);
    }

    void Start()
    {
        spawnPosition = transform.position;

        // Sets expiration lifetime
        Destroy(gameObject, maxTime);
    }

    private void Update()
    {
        if (maxDistance > 0)
        {
            float distance = Vector3.Distance(spawnPosition, transform.position);

            if (distance >= maxDistance)
            {
                Destroy(gameObject);
            }
        }
    }

    public virtual void LaunchProjectile(Vector3 direction, float force)
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        rb.AddForce(direction * force);
    }

    // Interaction with units and environments
    void OnTriggerEnter(Collider other)
    {
        Entity target = other.GetComponent<Entity>();

        // if the object hit was the person who shot it, don't destroy the projectile
        if (other.tag == "Floor" || (target != null && GetComponent<Hitbox>().owner == target))
        {
            return;
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        ProjectileManager.Instance.activeProjectiles.Remove(this);
    }

}
