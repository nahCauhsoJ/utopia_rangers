using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DefenseCore : MonoBehaviour
{
    public Transform pool_parent;
    public static ObjectPool<GameObject> projectile_pool;
    public GameObject projectile_prefab;

    void Awake()
    {
        if (projectile_pool == null) projectile_pool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(projectile_prefab,pool_parent),
            actionOnGet: (x) => x.SetActive(true),
            actionOnRelease: (x) => x.SetActive(false),
            actionOnDestroy: (x) => Destroy(x),
            collectionCheck: false,
            defaultCapacity: 50,
            maxSize: 200
        );
    }

    public static void SpawnSquirrelProjectile(Vector3 pos, Quaternion rot,
            DefenseProjectile.ProjectileData proj_data, DefenseBase source_defense = null)
    {
        projectile_pool.Get().GetComponent<DefenseProjectile>().Reuse(pos, rot, proj_data, source_defense);
    }
}
