using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseBase : MonoBehaviour
{
    public int max_level = 1;

    public CircleCollider2D interact_area;
    public AimArea aim_area;
    public AudioSource voice; // Sometimes we question, how can animals talk without a good vocal cord?

    // These will be modified from ActionWheelCore.
    public int current_upgrade;
    public int upgrade_1_type;

    protected EnemyBase target_enemy;

    void Awake() {UpdateLevel();}

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

    public virtual void HitEnemy(EnemyBase enemy, float dmg)
    {
        enemy.Damage(dmg);
    }

    public virtual List<EnemyBase> HitEnemyAoe(Vector3 pos, float radius, float dmg)
    {
        List<EnemyBase> enemies = new List<EnemyBase>();
        Collider2D[] victims = Physics2D.OverlapCircleAll(pos, radius, LayerMask.GetMask("Enemy"));
        foreach (var i in victims) enemies.Add(i.GetComponent<EnemyBase>());
        foreach (var i in enemies) i.Damage(dmg);
        return enemies;
    }

    // Some units need another update upon placing down.
    public virtual void OnPlace()
    {

    }

    // THE lazy function to simply refresh the unit from its upgrade states.
    // Do note that the derived class will DEFinitely override this.
    // Also note that we'll run this at the start as well. We're this lazy.
    public virtual void UpdateLevel()
    {

    }
}
