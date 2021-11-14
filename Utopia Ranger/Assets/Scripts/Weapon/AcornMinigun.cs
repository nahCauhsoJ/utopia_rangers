using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcornMinigun : WeaponBase
{
    public float knockback;
    public Vector2 bullet_scatter_angle; // In case you don't want a shotgun with sniper accuracy.

    public override void HitEnemy(EnemyBase enemy, RegularBullet.BulletData bullet)
    {
        base.HitEnemy(enemy, bullet);
        enemy.Knockback(knockback);
    }

    // Not using base this time.
    protected override void PEWPEW()
    {
        WeaponCore.SpawnRegularBullet(transform.position, ScatterBullet(transform.rotation, bullet_scatter_angle), bullet_data, this);
    }
}
