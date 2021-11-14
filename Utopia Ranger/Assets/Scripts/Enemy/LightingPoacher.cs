using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingPoacher : EnemyBase
{
    [Header("Lighting Squad Extras")]
    public int dash_ouch_needed; // The number of times to get hit before he dashes forward
    public float dash_progress; // An instant dash towards the base. This is in world units.

    int dash_ouch_counter;

    protected override void Awake()
    {
        base.Awake();
        enemy_type = EnemyType.Lighting;
    }

    public override void Damage(float dmg)
    {
        base.Damage(dmg);
        if (!dead) dash_ouch_counter++;
        if (dash_ouch_counter >= dash_ouch_needed)
        {
            dash_ouch_counter -= dash_ouch_needed;
            path_progress += dash_progress;
        }
    }
}
