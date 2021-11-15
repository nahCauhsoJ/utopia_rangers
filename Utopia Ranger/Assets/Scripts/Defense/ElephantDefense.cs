using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElephantDefense : DefenseBase
{
    public Transform trunk;
    public AudioSource hose_looping_sound; // Not voice? Oh. You owe me $5.

    [Header("Mud")]
    public SpriteRenderer mud_ball_sprite;
    public GameObject mud_splat_prefab;
    public float mud_dmg;
    public float mud_splat_radius;
    public float mud_fly_speed;
    public float mud_dry_time;
    float mud_fly_time_elapsed;
    Dictionary<GameObject,float> mud_splats = new Dictionary<GameObject,float>();

    [Header("Water & Lava")]
    public SpriteRenderer water_stream_sprite;
    public SpriteRenderer lava_stream_sprite;
    public BoxCollider2D stream_collider;
    public float stream_radius; // Yes, it's still a circlr even if it's a stream.
    public float water_dpt; // dpt stands for damage per tick
    public float lava_dpt;

    [Header("Peanut")]
    public SpriteRenderer peanut_pulse_sprite;
    public float peanut_radius;
    public float peanut_dmg;
    float peanut_dry_elapsed; // Let's hard-code to 1 second.

    [Header("Stats")]
    public float splat_pulse_rate; // This is for the mud splat and peanut splat.
    public float stream_tick_rate; // Time in seconds between each tick of damage.
    float splat_pulse_rate_left;
    float stream_tick_rate_left;

    Vector3 pulse_target_pos; // This gives a snapshot of where the object will travel to, then splat.

    int current_floor; // 0: Regular, 1: Water, 2: Lava

    // Basically only when it's first placed or placed again after moving.
    void OnEnable()
    {
        current_floor = 0;
        Collider2D water = Physics2D.OverlapCircle(transform.position, interact_area.radius, LayerMask.GetMask("Zone_Water"));
        if (water != null) current_floor = 1;
        Collider2D lava = Physics2D.OverlapCircle(transform.position, interact_area.radius, LayerMask.GetMask("Zone_Lava"));
        if (lava != null) current_floor = 2;
        StreamColliderUpdate(0);
        StreamFloorUpdate();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (target_enemy != null)
        {
            if (splat_pulse_rate_left > 0) splat_pulse_rate_left -= Time.deltaTime;
            if (splat_pulse_rate_left < 0) splat_pulse_rate_left = 0;
            if (splat_pulse_rate_left == 0) { PulseAttack(); splat_pulse_rate_left = splat_pulse_rate; }

            if (stream_tick_rate_left > 0) stream_tick_rate_left -= Time.deltaTime;
            if (stream_tick_rate_left < 0) stream_tick_rate_left = 0;
            if (stream_tick_rate_left == 0) { StreamAttack(); stream_tick_rate_left = stream_tick_rate; }
        } else {
            StreamColliderUpdate(0);
            if (hose_looping_sound.isPlaying) hose_looping_sound.Stop();
        }

        if (mud_ball_sprite.enabled)
        {
            mud_ball_sprite.transform.position = Vector3.MoveTowards(
                mud_ball_sprite.transform.position, pulse_target_pos, mud_fly_speed * Time.deltaTime);
            if (Vector3.Distance(mud_ball_sprite.transform.position,pulse_target_pos) < 0.01f) MudSplat();
        } else if (mud_splats.Count > 0) {
            List<GameObject> dried_splat = new List<GameObject>();
            foreach(var i in new List<GameObject>(mud_splats.Keys))
            {
                mud_splats[i] -= Time.deltaTime;
                if (mud_splats[i] <= 0) dried_splat.Add(i);
            }
            foreach (var i in dried_splat) Destroy(i);
        }
        if (peanut_pulse_sprite.enabled)
        {
            peanut_dry_elapsed += Time.deltaTime;
            if (peanut_dry_elapsed > 1f)
            {
                peanut_pulse_sprite.enabled = false;
                peanut_dry_elapsed = 0;
            }
        }
    }

    public override void HitEnemy(EnemyBase enemy, float dmg)
    { 
        base.HitEnemy(enemy,dmg);
        if (current_upgrade == 1 && upgrade_1_type == 1)
        {

        }
    }

    public void PulseAttack()
    {
        if (target_enemy == null) return;
        if (current_upgrade == 1 && upgrade_1_type != 1) return;
        if (current_upgrade == 0 && current_floor != 0) return;
        SetAimRotation();
        transform.rotation = aim_area.transform.rotation;
        pulse_target_pos = target_enemy.transform.position;
        if (current_upgrade == 0)
        {
            mud_ball_sprite.transform.position = trunk.position;
            mud_ball_sprite.enabled = true;
        } else PeanutSplat();
    }
    void MudSplat()
    {
        mud_ball_sprite.enabled = false;
        mud_splats[Instantiate(mud_splat_prefab,pulse_target_pos,Quaternion.identity)] = mud_dry_time;
        HitEnemyAoe(pulse_target_pos,mud_splat_radius, mud_dmg);
        Sound.Play(voice,Sound.main.mud,0.5f,Random.Range(0.7f,0.9f)); 
    }

    void PeanutSplat()
    {
        peanut_pulse_sprite.transform.position = pulse_target_pos;
        peanut_pulse_sprite.enabled = true;
        HitEnemyAoe(pulse_target_pos, peanut_radius, peanut_dmg);
        Sound.Play(voice,Sound.main.peanut_splat,1,Random.Range(0.9f,1.1f)); 
    }

    public void StreamAttack()
    {
        if (target_enemy == null) return;
        if (current_upgrade == 1 && upgrade_1_type == 1) return;
        if (current_upgrade == 0 && current_floor == 0) return;
        SetAimRotation();
        transform.rotation = aim_area.transform.rotation;
        StreamColliderUpdate(Vector3.Distance(target_enemy.transform.position,transform.position));
        if (current_floor == 1)
        {
            HitEnemyAoe(target_enemy.transform.position, stream_radius, water_dpt);
            if (!hose_looping_sound.isPlaying) { hose_looping_sound.pitch = 1f; hose_looping_sound.Play(); }
        }
        if (current_floor == 2)
        {
            HitEnemyAoe(target_enemy.transform.position, stream_radius, lava_dpt);
            if (!hose_looping_sound.isPlaying) { hose_looping_sound.pitch = 0.7f; hose_looping_sound.Play(); }
        }
    }
    // Length from center.
    // Also spammable.
    void StreamColliderUpdate(float stream_length)
    {
        if (stream_length <= interact_area.radius) { stream_collider.transform.localScale = Vector3.zero; return; }
        // This is basically the width of the donut
        float new_y_pos = (stream_length - interact_area.radius) / 2;
        stream_collider.transform.localPosition = Vector3.up * new_y_pos;
        // The mask sprite's dimension's original ratio is 1:4. Hence / 4.
        stream_collider.transform.localScale = new Vector3(1,new_y_pos / 4,1);
    }

    void StreamFloorUpdate()
    {
        if (current_floor == 1) { water_stream_sprite.enabled = true; lava_stream_sprite.enabled = false;}
        else if (current_floor == 2) { water_stream_sprite.enabled = false; lava_stream_sprite.enabled = true;}
    }

    public override void UpdateLevel()
    {
        base.UpdateLevel();
        StreamColliderUpdate(0);
        StreamFloorUpdate();
        switch (current_upgrade)
        {
            case 0:

                break;
            case 1:
                switch (upgrade_1_type)
                {
                    case 0:

                        break;
                    case 1:

                        break;
                }
                break;
        }
    }
}
