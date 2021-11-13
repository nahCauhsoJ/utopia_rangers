using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class WeaponCore : MonoBehaviour
{
    public Transform pool_parent;
    public static ObjectPool<GameObject> reg_bullet_pool;
    public GameObject regular_bullet_prefab;

    void Awake()
    {
        if (reg_bullet_pool == null) reg_bullet_pool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(regular_bullet_prefab,pool_parent),
            actionOnGet: (x) => x.SetActive(true),
            actionOnRelease: (x) => x.SetActive(false),
            actionOnDestroy: (x) => Destroy(x),
            collectionCheck: false,
            defaultCapacity: 50,
            maxSize: 200
        );
    }

    public static void SpawnRegularBullet(Vector3 pos, Quaternion rot, RegularBullet.BulletData b, WeaponBase source_weapon = null)
    {
        reg_bullet_pool.Get().GetComponent<RegularBullet>().Reuse(pos, rot, b, source_weapon);
    }
}
