using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "Main Scene";

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(gameSceneName);
        // SaveSystem will load automatically in the scene
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
