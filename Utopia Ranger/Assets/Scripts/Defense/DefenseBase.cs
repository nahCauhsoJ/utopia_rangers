using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseBase : MonoBehaviour
{
    public int max_level = 1;

    public Collider2D interact_area;
    public AimArea aim_area;

    // These will be modified from ActionWheelCore.
    public int current_upgrade{get;set;}
    public int upgrade_1_type{get;set;}

    protected EnemyBase target_enemy;

    void Awake() {UpdateLevel();}

    protected virtual void FixedUpdate()
    {
        target_enemy = aim_area.GetFrontmostEnemy();
    }

    protected void SetAimRotation()
    {
        Vector3 rot = target_enemy.transform.position - transform.position;
        rot.Normalize();
        aim_area.transform.up = rot;
    }

    public virtual void HitEnemy(EnemyBase enemy, DefenseProjectile.ProjectileData proj)
    {
        enemy.Damage(proj.damage);
    }

    // THE lazy function to simply refresh the unit from its upgrade states.
    // Do note that the derived class will DEFinitely override this.
    // Also note that we'll run this at the start as well. We're this lazy.
    public virtual void UpdateLevel()
    {

    }
}
