using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseProjectile : MonoBehaviour
{
    const float proj_lifetime = 3f;
    public float lifetime{get; private set;}

    public ProjectileData proj;
    public SpriteRenderer sprite;
    public DefenseBase source;

    public void Reuse(Vector3 pos, Quaternion rot, ProjectileData proj_data, DefenseBase source_defense = null)
    {
        transform.position = pos;
        transform.rotation = rot;
        sprite.sprite = proj_data.img;
        proj = proj_data;
        source = source_defense;
    }

    void Return()
    {
        lifetime = 0;
        DefenseCore.projectile_pool.Release(gameObject);
    }

    void FixedUpdate()
    {
        lifetime += Time.deltaTime;
        if (lifetime > proj_lifetime) Return();

        Vector2 dist = (Vector2) transform.up * proj.move_speed * Time.fixedDeltaTime;
        RaycastHit2D hit = Physics2D.Linecast((Vector2)transform.position,
            (Vector2)transform.position + dist,
            LayerMask.GetMask("Enemy"));
        if (hit.collider != null && DidHitEnemy(hit.collider)) Return();
        transform.position += (Vector3) dist;
    }

    protected virtual bool DidHitEnemy(Collider2D c)
    {
        EnemyBase enemy = c.GetComponent<EnemyBase>();
        if (enemy == null) return false;
        if (source == null) enemy.Damage(proj.damage);
        else source.HitEnemy(enemy, proj);
        return true;
    }

    // All projectiles have at least these variables.
    // For anything special, modify it from somewhere else. Remember that we have the source here.
    [System.Serializable]
    public class ProjectileData
    {
        public ProjectileData(float move_speed, float damage, Sprite img = null)
        {
            this.move_speed = move_speed;
            this.damage = damage;
            this.img = img;
        }
        public float move_speed;
        public float damage;
        public Sprite img;
    }
}
