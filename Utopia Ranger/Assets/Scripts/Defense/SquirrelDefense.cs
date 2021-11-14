using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquirrelDefense : DefenseBase
{
    public DefenseProjectile.ProjectileData projectile_data;
    public Sprite squirrel_stone_img;
    public Sprite squirrel_acorn_img;
    public Sprite squirrel_poop_img;

    public float throw_rate; // Unit in throws per second
    float throw_cd_left;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (throw_cd_left > 0) throw_cd_left -= Time.deltaTime;
        if (throw_cd_left < 0) throw_cd_left = 0;

        if (throw_cd_left == 0) ProjectileThrow();
    }

    void ProjectileThrow()
    {
        if (target_enemy == null) return;

        throw_cd_left = throw_rate;
        SetAimRotation();
        DefenseCore.SpawnSquirrelProjectile(transform.position, aim_area.transform.rotation, projectile_data, this);
    }

    public override void HitEnemy(EnemyBase enemy, DefenseProjectile.ProjectileData proj)
    { 
        base.HitEnemy(enemy,proj);
        // This is poop's slow effect.
        if (current_upgrade == 1 && upgrade_1_type == 1)
        {   // It says squirrel poop, but it just means poop thrown by the squirrels, it's actually hyena poop.
            DebuffBase debuff = DebuffBase.GetDebuff("squirrel_poop",enemy.debuffs);
            if (debuff == null) enemy.debuffs.Add(new DebuffSlow(enemy, 3f, -0.5f, "squirrel_poop")); 
            else debuff.Extend(3f);
        }
    }

    public override void UpdateLevel()
    {
        base.UpdateLevel();
        switch (current_upgrade)
        {
            case 0:
                projectile_data.damage = 20f;
                projectile_data.img = squirrel_stone_img;
                break;
            case 1:
                switch (upgrade_1_type)
                {
                    case 0:
                        projectile_data.damage = 50f;
                        projectile_data.img = squirrel_acorn_img;
                        break;
                    case 1:
                        projectile_data.damage = 20f;
                        projectile_data.img = squirrel_poop_img;
                        break;
                }
                break;
        }
    }
}
