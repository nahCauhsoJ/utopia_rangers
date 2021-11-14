using UnityEngine;
using UnityEngine.UI;

public class HUDCore : MonoBehaviour
{
    public Text infestation_text;
    public Text greed_text;

    void Update()
    {
        infestation_text.text = string.Format("Infestation: {0}/{1}", MapCore.main.current_wave+1, MapCore.main.waves.Count);
        greed_text.text = DefenseCore.main.greed.ToString();
    }
}
