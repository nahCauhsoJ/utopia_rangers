using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCore : MonoBehaviour
{
    public static MapCore main;

    public float base_health = 100f;
    public bool game_on;

    public AudioSource voice; // Sometimes we question, how can an- wait this is a frickin' floor.
    public List<Paths> available_paths;
    public List<Collider2D> place_zones;
    public Dictionary<int,PathStats> path_data = new Dictionary<int,PathStats>(); // The Count should be parallel to available_paths

    public List<GameObject> waves;
    Dictionary<int,WaveStatus> waves_status = new Dictionary<int,WaveStatus>();
    Dictionary<int,EnemyBase[]> wave_enemies = new Dictionary<int,EnemyBase[]>();
    public int current_wave{get; private set;} // Starts at 0, not 1.

    public float next_wave_delay;
    public bool next_wave_waiting{get; private set;}
    public float next_wave_elapsed{get; private set;}

    void Awake()
    {
        main = this;
        for (var i = 0; i < available_paths.Count; i++) path_data[i] = new PathStats(available_paths[i].path_points);
        for (var i = 0; i < waves.Count; i++)
        {
            waves_status[i] = new WaveStatus();
            waves[i].SetActive(true);
            wave_enemies[i] = waves[i].GetComponentsInChildren<EnemyBase>();
            foreach (var j in wave_enemies[i]) j.battle_ready = true;
            waves[i].SetActive(false);
        }
    }

    void Start()
    {
        game_on = true;
        current_wave = -1; // It's safe. I know what I'm doing.
        next_wave_waiting = true;
    }

    void Update()
    {
        if (next_wave_waiting)
        {
            next_wave_elapsed += Time.deltaTime;
            if (next_wave_elapsed >= next_wave_delay)
            {
                next_wave_elapsed = 0;
                next_wave_waiting = false;
                current_wave++;
                SpawnWave(current_wave);
            }
        }
    }

    void FixedUpdate()
    {
        if (game_on)
        {
            if (base_health <= 0) LoseLevel();
            else if (!next_wave_waiting && waves[current_wave].activeSelf)
            {
                int enemies_left = 0;
                foreach (var i in wave_enemies[current_wave])
                {
                    if (i.battle_ready && !i.dead)
                    {
                        i.path_progress += (i.move_speed > 0 ? i.move_speed : 0) * Time.deltaTime;
                        i.SetPathPos();
                        if (i.rb.position == path_data[i.path_index].path_nodes[path_data[i.path_index].path_nodes.Count-1]) DamageBase(i);
                        enemies_left++;
                    }
                }
                waves_status[current_wave].enemy_count = enemies_left;
                waves_status[current_wave].cleared = enemies_left == 0;
                if (waves_status[current_wave].cleared) EndWave();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        for (var i = 0; i < available_paths.Count; i++)
        {
            float ratio = (float)i/available_paths.Count;
            Vector2 ratio_offset = new Vector2(ratio,ratio) * 0.2f;
            Gizmos.color = new Color(ratio,ratio,1,1);
            for (var j = 0; j < available_paths[i].path_points.Count-1; j++)
                Gizmos.DrawLine(available_paths[i].path_points[j] + ratio_offset,available_paths[i].path_points[j+1] + ratio_offset);
        }
        foreach (var i in place_zones)
        {
            Gizmos.color = new Color(1f,1f,0.5f,0.5f);
            Gizmos.DrawCube(i.transform.position,i.bounds.size);
        }
    }

    public void SpawnWave(int wave)
    {
        current_wave = wave;
        waves[current_wave].SetActive(true);
        Sound.Play(voice,Sound.main.wave_start,0.5f,1f); 
    }

    public void EndWave()
    {
        waves[current_wave].SetActive(false);
        if (current_wave >= waves.Count - 1) WinLevel();
        else { next_wave_waiting = true; }
    }

    public void DamageBase(EnemyBase enemy)
    {
        base_health -= enemy.damage;
        enemy.Die(true);
        Sound.Play(voice,Sound.main.punch,0.5f,Random.Range(0.9f,1.1f)); 
    }

    public void LoseLevel()
    {
        HUDCore.main.game_over.SetActive(true);
        game_on = false;
    }

    public void WinLevel()
    {
        print("Win");
        game_on = false;
    }

    public class WaveStatus
    {
        public bool spawned;
        public bool cleared;
        public int enemy_count;
    }

    public class PathStats
    {
        public List<Vector2> path_nodes{get; private set;}
        List<float> path_lengths = new List<float>(); // It should always have 1 less item than path_nodes.
        public float total_length{get; private set;}

        public PathStats(List<Vector2> path_nodes)
        {
            this.path_nodes = path_nodes;
            total_length = 0f;
            path_lengths.Clear();
            for (var i = 0; i < path_nodes.Count - 1; i++)
            {
                path_lengths.Add(Vector2.Distance(path_nodes[i],path_nodes[i+1]));
                total_length += path_lengths[i];
            }
        }

        public Vector2 GetPathPos(float current_length)
        {
            if (path_nodes.Count == 0) return Vector2.zero;

            if (current_length >= total_length) return path_nodes[path_nodes.Count-1];
            else if (current_length < 0) return path_nodes[0];
            else
            {
                float current_length_check = 0f;
                for (var i = 0; i < path_nodes.Count - 1; i++)
                {
                    if (current_length < current_length_check + path_lengths[i])
                    return Vector2.Lerp(path_nodes[i],path_nodes[i+1],
                        (current_length - current_length_check) / path_lengths[i]);
                    else current_length_check += path_lengths[i];
                }
                return Vector2.zero; // It shouldn't reach here.
            }
        } 
    }
}
