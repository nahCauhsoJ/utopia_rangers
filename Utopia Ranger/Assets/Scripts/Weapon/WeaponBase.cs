using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    public RegularBullet.BulletData bullet_data;
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

    // Does nothing if the round isn't available
    void PEW()
    {
        if (automatic)
        {
            if (round_interval_left == 0) { PEWPEW(); round_interval_left = round_interval; }
        } else {
            if (round_cooldown_left == 0) { PEWPEW(); round_cooldown_left = round_cooldown; }
        }
    }

    // This is the class we want to derive if the weapon fires a different type of bullet
    protected virtual void PEWPEW()
    {
        WeaponCore.SpawnRegularBullet(transform.position, transform.rotation, bullet_data, this);
    }
}
