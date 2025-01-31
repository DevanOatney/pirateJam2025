using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    // Singleton
    private static ProjectileManager instance;
    public static ProjectileManager Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }
            else
            {
                instance = FindFirstObjectByType<ProjectileManager>();
                return instance;
            }
        }
    }

    public List<ProjectileBase> activeProjectiles;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
