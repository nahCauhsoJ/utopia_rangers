using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffSlow : DebuffBase
{
    // If speed_drop is positive, it detects flat move_speed.
    //      If negative, it becomes a percent drop of the total speed, ranging from 0.0 to 1.0.
    public DebuffSlow(EnemyBase victim, float duration, float speed_drop, string name = "Slow") :
        base(victim, duration, name: name)
    {
        this.speed_drop = speed_drop;
    }

    float speed_drop;

    public override void OnActivate()
    {
        if (speed_drop < 0) speed_drop = victim.move_speed * (1+speed_drop);
        victim.move_speed -= speed_drop;
    }

    public override void OnWearOff()
    {
        victim.move_speed += speed_drop;
    }
}
