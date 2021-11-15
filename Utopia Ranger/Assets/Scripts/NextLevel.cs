using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    public PlayableDirector director;

    void OnEnable()
    {
        director.Play();
    }

    public void NextLevelScene(string lvl_name)
    {
        SceneManager.LoadScene(lvl_name);
    }
}
