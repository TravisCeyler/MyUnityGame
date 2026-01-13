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
        if (SaveSystem.Instance == null)
        {
            Debug.LogError("SaveSystem not found!");
            return;
        }

        // Let SaveSystem handle everything
        SaveSystem.Instance.LoadGameFromMenu();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
