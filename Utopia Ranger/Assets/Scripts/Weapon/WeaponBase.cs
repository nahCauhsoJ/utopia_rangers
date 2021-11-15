using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    public RegularBullet.BulletData bullet_data;
    public AudioSource voice; // Sometimes we question, how can YOU JUST shut UP. (Hey, but guns can talk.)
    public int magazine; // If it exceeds 999 (i.e. >= 1000), it's unlimited bullets.
    public bool automatic; // If automatic, holding the shoot button will spam the bullets.
    public float round_cooldown; // Whether it's automatic or not, if this is > 0, it cannot shoot.
    public float round_interval; // If automatic, this is basically the cooldown between bullets.

    protected float round_cooldown_left;
    protected float round_interval_left;

    protected bool is_held;

    public virtual void OnShootStart()
    {
        is_held = true;
        PEW();
    }

    public virtual void OnShootHold()
    {
        PEW();
    }

    public virtual void OnShootEnd()
    {
        is_held = false;
    }

    protected virtual void Update()
    {
        if (is_held) OnShootHold();
        if (automatic)
        {
            if (round_interval_left > 0) round_interval_left -= Time.deltaTime;
            if (round_interval_left < 0) round_interval_left = 0;
        }
        if (round_cooldown_left > 0) round_cooldown_left -= Time.deltaTime;
        if (round_cooldown_left < 0) round_cooldown_left = 0;
    }

    // angle range: negative degree to positive, starting at 0. e.g. (-10,10) scatters bullet for 20 degrees, anywhere.
    protected Quaternion ScatterBullet(Quaternion orig_rot, Vector2 angle_range)
    {
        if (angle_range.x == angle_range.y) return orig_rot;
        return orig_rot * Quaternion.AngleAxis(Random.Range(angle_range.x,angle_range.y), Vector3.forward);
    }

    // Does nothing if the round isn't available
    void PEW()
    {
        if (!MapCore.main.game_on) return;
        if (automatic)
        {
            if (round_interval_left == 0) { PEWPEW(); round_interval_left = round_interval; }
        } else {
            if (round_cooldown_left == 0) { PEWPEW(); round_cooldown_left = round_cooldown; }
        }
    }

    // This is the class we want to derive if the weapon does something else than firing a bullet.
    protected virtual void PEWPEW()
    {
        WeaponCore.SpawnRegularBullet(transform.position, transform.rotation, bullet_data, this);
    }

    public virtual void HitEnemy(EnemyBase enemy, RegularBullet.BulletData bullet)
    {
        enemy.Damage(bullet.damage);
    }
}
