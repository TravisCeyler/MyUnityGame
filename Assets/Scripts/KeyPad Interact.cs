using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeypadTrigger : MonoBehaviour
{
    public GameObject keypadUI;
    public TextMeshProUGUI displayText; // or use Text if you’re not using TMP
    public string correctCode = "1234";

    private bool playerNearby = false;
    private string enteredCode = "";

    void Update()
    {
        // Detect player pressing E near keypad
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            ToggleKeypad(true);
        }
    }

    public void ToggleKeypad(bool show)
    {
        keypadUI.SetActive(show);
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = show;
        Time.timeScale = show ? 0f : 1f; // Pause game
    }

    // Called by buttons in the UI
    public void AddDigit(string digit)
    {
        enteredCode += digit;
        displayText.text = enteredCode;
    }

    public void ClearCode()
    {
        enteredCode = "";
        displayText.text = "";
    }

    public void EnterCode()
    {
        if (enteredCode == correctCode)
        {
            Debug.Log("✅ Correct Code! Event triggered!");
            // You can call any event here, like opening a door
        }
        else
        {
            Debug.Log("❌ Wrong Code!");
        }

        ClearCode();
        ToggleKeypad(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            ToggleKeypad(false);
        }
    }
}
