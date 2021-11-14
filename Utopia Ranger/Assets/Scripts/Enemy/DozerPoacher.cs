using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DozerPoacher : EnemyBase
{
    //[Header("Dozer Squad Extras")]

    protected override void Awake()
    {
        base.Awake();
        enemy_type = EnemyType.Dozer;
    }
}
