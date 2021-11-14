using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimArea : MonoBehaviour
{
    public CircleCollider2D aim_area;

    public List<Collider2D> enemies_in_range{get; private set;} = new List<Collider2D>();
    Dictionary<Collider2D,EnemyBase> enemies_data = new Dictionary<Collider2D,EnemyBase>();
    void OnTriggerEnter2D(Collider2D c) { enemies_in_range.Add(c); enemies_data[c] = c.GetComponent<EnemyBase>(); }
    void OnTriggerExit2D(Collider2D c) { enemies_in_range.Remove(c); enemies_data.Remove(c); }

    public EnemyBase GetFrontmostEnemy()
    {
        float most_progress = -1f;
        Collider2D the_enemy = null;
        foreach (var c in enemies_in_range)
        {
            if (enemies_data[c].path_progress > most_progress)
            {
                most_progress = enemies_data[c].path_progress;
                the_enemy = c;
            }
        }
        return the_enemy == null ? null : enemies_data[the_enemy];
    }
}
