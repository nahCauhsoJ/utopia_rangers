using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Even if it says "regular" instead of "base", this is technically a base class, but actually usable.
public class RegularBullet : MonoBehaviour
{
    const float bullet_lifetime = 3f;

    public BulletData bullet;
    public SpriteRenderer sprite;
    public WeaponBase source;

    public float lifetime{get; private set;}

    // We're doing pooling, so yeah.
    public void Reuse(Vector3 pos, Quaternion rot, BulletData b, WeaponBase source_weapon = null)
    {
        transform.position = pos;
        transform.rotation = rot;
        sprite.sprite = b.img;
        bullet = b;
        source = source_weapon;
    }

    void Return()
    {
        lifetime = 0;
        WeaponCore.reg_bullet_pool.Release(gameObject);
    }

    void FixedUpdate()
    {
        lifetime += Time.deltaTime;
        if (lifetime > bullet_lifetime) Return();

        Vector2 dist = (Vector2) transform.up * bullet.move_speed * Time.fixedDeltaTime;
        RaycastHit2D hit = Physics2D.Linecast((Vector2)transform.position,
            (Vector2)transform.position + dist,
            LayerMask.GetMask("Enemy"));
        if (hit.collider != null && DidHitEnemy(hit.collider)) Return();
        transform.position += (Vector3) dist;
    }

    bool DidHitEnemy(Collider2D c)
    {
        EnemyBase enemy = c.GetComponent<EnemyBase>();
        if (enemy == null) return false;
        if (source == null) enemy.Damage(bullet.damage);
        else source.HitEnemy(enemy,bullet);
        return true;
    }

    [System.Serializable]
    public class BulletData
    {
        public BulletData(float move_speed, float damage, Sprite img = null)
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
