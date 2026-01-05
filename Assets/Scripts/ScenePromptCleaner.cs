using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePromptCleaner : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Destroy any leftover prompts from previous scene
        var oldInteract = GameObject.Find("InteractPrompt");
        if (oldInteract != null) Destroy(oldInteract);

        var oldDialogue = GameObject.Find("DialoguePanel");
        if (oldDialogue != null) Destroy(oldDialogue);
    }
}
