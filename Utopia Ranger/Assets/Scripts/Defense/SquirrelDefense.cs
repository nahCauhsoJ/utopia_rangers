using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquirrelDefense : DefenseBase
{
    public DefenseProjectile.ProjectileData acorn_data;

    public float throw_rate; // Unit in throws per second
    float throw_cd_left;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (throw_cd_left > 0) throw_cd_left -= Time.deltaTime;
        if (throw_cd_left < 0) throw_cd_left = 0;

        if (throw_cd_left == 0) AcornThrow();
    }

    void AcornThrow()
    {
        if (target_enemy == null) return;

        throw_cd_left = throw_rate;
        SetAimRotation();
        DefenseCore.SpawnSquirrelProjectile(transform.position, aim_area.transform.rotation, acorn_data, this);
    }
}
