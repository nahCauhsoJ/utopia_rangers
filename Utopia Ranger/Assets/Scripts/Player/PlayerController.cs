using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public Collider2D col;
    public SpriteRenderer sprite;

    Camera cam;
    Vector3 mouse_world_pos;

    public float start_move_speed;
    public float move_speed{get;set;}

    public WeaponBase current_weapon;

    // This chunk here is basically how to initiate the new input system.
    Controls pc_core;
    Controls.PlayerActions pc_map;
    Vector2 player_move_dir;
    bool player_shoot;
    void OnEnable() {pc_map.Enable();}
    void OnDisable() {pc_map.Disable();}
    void InputSetup()
    {
        pc_core = new Controls();
        pc_map = pc_core.Player;

        pc_map.Move.performed += ctx => player_move_dir = ctx.ReadValue<Vector2>();
        pc_map.ShootStart.performed += _ => current_weapon.OnShootStart();
        pc_map.ShootEnd.performed += _ => current_weapon.OnShootEnd();
    }

    void Awake()
    {
        cam = Camera.main;
        InputSetup();
        move_speed = start_move_speed;
    }

    void Update()
    {
        mouse_world_pos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouse_world_pos = new Vector3(mouse_world_pos.x,mouse_world_pos.y,transform.position.z);
        transform.up = (mouse_world_pos - transform.position) / Vector2.Distance(mouse_world_pos,transform.position);
    }
    
    void FixedUpdate()
    {
        rb.velocity = player_move_dir == Vector2.zero ? Vector2.zero : player_move_dir * move_speed * Time.deltaTime;
    }
}
