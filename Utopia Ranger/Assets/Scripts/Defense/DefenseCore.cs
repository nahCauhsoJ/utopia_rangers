using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DefenseCore : MonoBehaviour
{
    public static DefenseCore main;

    public Transform pool_parent;
    public static ObjectPool<GameObject> projectile_pool;
    public GameObject projectile_prefab;


    public List<DefenseData> def_data;

    public int greed;

    void Awake()
    {
        main = this;
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

    [System.Serializable]
    public class DefenseData
    {
        public int unit_id;
        public string unit_name;
        public GameObject unit_prefab;
        public DefenseCosts cost;
        public Vector2 unit_size;
    }
    [System.Serializable]
    public class DefenseCosts
    {
        public int lvl_0;
        public List<int> lvl_1;
        //public List<int> upgrade_2_cost;
    }
    public static DefenseData GetDefData(int unit_id)
    {
        foreach (var i in main.def_data) if (i.unit_id == unit_id) return i;
        return null;
    }
    public static DefenseData GetDefDataFromInstance(DefenseBase def_obj)
    {
        if (def_obj == null) return null;
        if (def_obj.GetComponent<SquirrelDefense>() != null) return main.def_data[0];
        if (def_obj.GetComponent<ElephantDefense>() != null) return main.def_data[1];
        return null;
    }

    public static int GetTotalCost(DefenseBase def)
    {
        DefenseData def_data = GetDefDataFromInstance(def);
        int total_greed_salvaged = def_data.cost.lvl_0;
        if (def.current_upgrade == 1)
            total_greed_salvaged += def_data.cost.lvl_1[def.upgrade_1_type];
        return total_greed_salvaged;
    }
}
