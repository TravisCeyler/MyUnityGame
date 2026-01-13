using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class HintManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject hintPanel;
    public TMP_Text hintText;

    [Header("Input")]
    public KeyCode hintKey = KeyCode.I;

    private bool hintOpen = false;

    // Scene → Hint mapping
    private Dictionary<string, string> sceneHints = new Dictionary<string, string>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // 🔹 Add your scene hints here
        sceneHints.Add("MainMenu", "Press Start to begin your journey.");
        sceneHints.Add("Main Scene", "You should speak with the Boss. to get first mission");
        sceneHints.Add("Midieval", "Find and talk to five townfolk for the code");
        sceneHints.Add("Ancient Egypt", "Find the King for you journey.");
        sceneHints.Add("Space Age", "Find the hidden buttons and find the hiddent portal");
    }

    private void Update()
    {
        if (Input.GetKeyDown(hintKey))
        {
            ToggleHint();
        }
    }

    void ToggleHint()
    {
        hintOpen = !hintOpen;
        hintPanel.SetActive(hintOpen);

        if (hintOpen)
            UpdateHint();
    }

    void UpdateHint()
{
    string scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

    // MAIN SCENE
    if (scene == "Main Scene")
    {
        if (!GameProgress.Instance.talkedToBoss)
        {
            hintText.text = "You should speak with the boss. He awaits nearby.";
        }
        else
        {
            hintText.text = "The boss has given you a task. Carry it out. By finding the portal";
        }
        return;
    }

    if (scene == "Midieval")
    {
        int[] code = GameProgress.Instance.medievalCode;

        // Ensure array has 5 elements
        if (code == null || code.Length != 5)
        {
            Debug.LogWarning("medievalCode array is not set correctly in GameProgress!");
            code = new int[5] { -1, -1, -1, -1, -1 };
        }

        // Build code string (show ? for missing digits)
        string codeStr = "";
        bool allFound = true;

        for (int i = 0; i < 5; i++)
        {
            int digit = (i < code.Length) ? code[i] : -1;

            if (digit == -1)
            {
                codeStr += "?";
                allFound = false;
            }
            else
            {
                codeStr += digit.ToString();
            }

            if (i < 4) // add space between digits
                codeStr += " ";
        }

        // Build final hint with new line if all found
        string hint = "Code: " + codeStr;

        if (allFound)
        {
            hint += "\nAll numbers gathered! Go to the church and enter the code.";
        }

        hintText.text = hint;
        return;
    }

    if (scene == "Ancient Egypt")
    {
        if (!GameProgress.Instance.talkedToKing)
        {
            hintText.text = "You should speak with the King. He awaits for you.";
        }
        else
        {
            hintText.text = "Find the Kings lost tresured item";
        }
        return;
    }

    if (scene == "Space Age")
    {
        hintText.text = "Find the hidden buttons that activate an hidden door";
    }
    return;




    // FALLBACK
    hintText.text = "Explore the area carefully.";
}

}
