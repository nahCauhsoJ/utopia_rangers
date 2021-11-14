using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseBase : MonoBehaviour
{
    public Collider2D interact_area;
    public AimArea aim_area;

    public int current_upgrade;

    protected EnemyBase target_enemy;

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
}
