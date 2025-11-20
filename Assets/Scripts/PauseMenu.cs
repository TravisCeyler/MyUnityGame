using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuRoot;   // Object tagged PauseMenu
    public GameObject pauseMenuUI;     // PauseMenuUI panel

    public static bool GameIsPaused = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (GameIsPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        GameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;
        GameIsPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        pauseMenuRoot = GameObject.FindGameObjectWithTag("PauseMenu");

        if (pauseMenuRoot == null)
        {
            Debug.LogError("PauseMenuRoot not found! Add the PauseMenu tag to your UI root.");
            return;
        }

        pauseMenuUI = pauseMenuRoot.transform.Find("PauseMenuUI")?.gameObject;

        if (pauseMenuUI == null)
        {
            Debug.LogError("PauseMenuUI child not found under PauseMenuRoot!");
            return;
        }

        pauseMenuUI.SetActive(false);

        // Auto connect button functions
        ConnectButtons();

        // Reset game state
        Time.timeScale = 1f;
        GameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("PauseMenu UI restored for scene: " + scene.name);
    }

    private void ConnectButtons()
    {
        Button resumeBtn = pauseMenuUI.transform.Find("ResumeButton")?.GetComponent<Button>();
        Button saveBtn = pauseMenuUI.transform.Find("SaveButton")?.GetComponent<Button>();
        Button loadBtn = pauseMenuUI.transform.Find("LoadButton")?.GetComponent<Button>();
        Button menuBtn = pauseMenuUI.transform.Find("MainMenuButton")?.GetComponent<Button>();
        Button quitBtn = pauseMenuUI.transform.Find("QuitButton")?.GetComponent<Button>();

        if (resumeBtn != null)
        {
            resumeBtn.onClick.RemoveAllListeners();
            resumeBtn.onClick.AddListener(Resume);
        }

        if (saveBtn != null)
        {
            saveBtn.onClick.RemoveAllListeners();
            saveBtn.onClick.AddListener(SaveGame);
        }

        if (loadBtn != null)
        {
            loadBtn.onClick.RemoveAllListeners();
            loadBtn.onClick.AddListener(LoadGame);
        }

        if (menuBtn != null)
        {
            menuBtn.onClick.RemoveAllListeners();
            menuBtn.onClick.AddListener(ReturnToMainMenu);
        }

        if (quitBtn != null)
        {
            quitBtn.onClick.RemoveAllListeners();
            quitBtn.onClick.AddListener(QuitGame);
        }

        Debug.Log("PauseMenu buttons linked.");
    }

    // ----------------------------------------------------
    // ✔ ADDED BACK — SAVE & LOAD FUNCTIONS
    // ----------------------------------------------------

    public void SaveGame()
    {
        Debug.Log("Saving game...");
        SaveSystem.Instance.SaveGame();
    }

    public void LoadGame()
    {
        Debug.Log("Loading game...");
        SaveSystem.Instance.LoadGame();
        Resume();  // close menu after loading
    }

    // ----------------------------------------------------

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
