using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class HUDCore : MonoBehaviour
{
    public static HUDCore main;

    public Text infestation_text;
    public Text greed_text;
    public Text thinair_text;

    public PlayableDirector director;
    // 0-2: fade in from black
    // 3-5: fade out to black
    // 7-9: fade in from white
    // 10-12 fade out to white

    public GameObject game_over;

    void Awake()
    {
        main = this;
    }

    void Start()
    {
        FadeIn();
    }

    void Update()
    {
        string wait_time_text = "";
        if (MapCore.main.next_wave_waiting) wait_time_text = string.Format(" ({0}s)",Mathf.CeilToInt(5f-MapCore.main.next_wave_elapsed));
        infestation_text.text = string.Format("Infestation: {0}/{1}{2}", MapCore.main.current_wave+1, MapCore.main.waves.Count,wait_time_text);
        greed_text.text = string.Format("Greed: {0}",DefenseCore.main.greed.ToString());
        float clamped_hp = MapCore.main.base_health;
        if (clamped_hp < 0) clamped_hp = 0;
        thinair_text.text = string.Format("{0}/100",clamped_hp.ToString());
    }

    // Don't mind this stupid fading method. Just playin' with the Timeline package.
    public void FadeIn(bool white = false)
    {
        if (white) {director.initialTime = 7f; director.Play();}
        else {director.initialTime = 0f; director.Play();}
        StartCoroutine(PauseAfter());
    }

    public void FadeOut(bool white = false)
    {
        if (white) {director.initialTime = 10f; director.Play();}
        else {director.initialTime = 3f; director.Play();}
        StartCoroutine(PauseAfter());
    }

    IEnumerator PauseAfter()
    {
        yield return new WaitForSeconds(2f);
        director.Pause();
    }


}
