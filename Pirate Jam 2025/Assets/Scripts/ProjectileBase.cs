using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    #region Properties

    Rigidbody rb;

    [SerializeField] float lifeTime = 5f; 

    #endregion Properties

    void Awake()
    {
        ProjectileManager.Instance.activeProjectiles.Add(this);

        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // Sets expiration lifetime
        Destroy(gameObject, lifeTime);
    }
    public virtual void LaunchProjectile(Vector3 direction, float force)
    {
        rb.AddForce(direction * force);
    }

    // Interaction with units and environments
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            
            // Destroy self if collided with something else
            Destroy(gameObject);
        }
    }

}
