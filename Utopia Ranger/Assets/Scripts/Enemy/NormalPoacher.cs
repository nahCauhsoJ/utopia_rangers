using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalPoacher : EnemyBase
{
    protected override void Awake()
    {
        base.Awake();
        enemy_type = EnemyType.Normal;
    }
}
