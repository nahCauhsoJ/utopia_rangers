using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public PlayableDirector director;

    void OnEnable()
    {
        director.Play();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("title");
    }
}
