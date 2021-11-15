using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public EnemyType enemy_type{get; protected set;}
    public enum EnemyType
    {
        None,
        Normal,
        Lighting,
        Dozer
    }
    [Header("Enemy Components")]
    public Rigidbody2D rb;
    public Collider2D col;
    public SpriteRenderer sprite;
    public AudioSource voice; // Sometimes we question, how can an- oh wait, they're human.

    // The enemy has to be active at the start to be recorded by MapCore, but
    //      we don't want to trigger the spawn mechanics. Hence, enemies only actually spawn
    //      if this is set to true, next time the GmeObject is active.
    [HideInInspector] public bool battle_ready;

    [Header("Enemy Spawn Conditions")]
    public float spawn_delay;

    [Header("Enemy Stats")]
    public float start_hp;
    public float hp{get;set;}
    public bool dead{get;set;}

    public float start_move_speed;
    public float move_speed{get;set;}
    public float damage;
    public int greed_worth;

    public List<DebuffBase> debuffs{get;set;} = new List<DebuffBase>();

    [Header("Path-related")]
    public int path_index; // Reference path_data of MapCore for the correct path. Not checking index overflow.
    public float path_progress{get; set;} // The number is in terms of world units. Actual position will be calculated by SetPathPos()]

    [Header("Modifiers for effects")]
    public float knockback_reduction;

    Vector2 orig_pos;

    //public List<>

    protected virtual void Awake() {orig_pos = rb.position;}
    protected virtual void Start() {}
    protected virtual void OnEnable() {if (battle_ready) StartCoroutine(Spawn(spawn_delay));}

    protected virtual void Update()
    {
        DebuffBase.UpdateDebuffs(debuffs, Time.deltaTime);
        transform.Rotate(transform.rotation.eulerAngles + Vector3.forward * 20f * Time.deltaTime);
    }

    IEnumerator Spawn(float delay)
    {
        rb.position = orig_pos;
        yield return new WaitForSeconds(delay);
        hp = start_hp;
        move_speed = start_move_speed;
        dead = false;
        sprite.enabled = true;
        col.enabled = true;
    }

    // If despawn is true, it means enemy reached the base, not being killed.
    // Note that battle_ready should still be true, since MapCore has the data.
    public void Die(bool despawn = false)
    {
        hp = 0;
        start_move_speed = 0;
        dead = true;
        sprite.enabled = false;
        col.enabled = false;
        if (!despawn)
        {
            DefenseCore.main.greed += greed_worth;
            Sound.Play(voice,Sound.main.poacher_death,0.3f,Random.Range(0.8f,1.3f)); 
        }
    }

    public virtual void Damage(float dmg)
    {
        hp -= dmg;
        if (hp <= 0) Die();
    }

    // This calculates and sets the enemy position based on the path data and path_progress.
    public void SetPathPos()
    {
        rb.position = MapCore.main.path_data[path_index].GetPathPos(path_progress);
    }

    public void Knockback(float oof)
    {
        path_progress -= oof * (1 - knockback_reduction);
        if (path_progress < 0) path_progress = 0;
    }
}
