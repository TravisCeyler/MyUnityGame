using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.AI;   // NEW – for stopping the NPC

public class NPCInteraction : MonoBehaviour
{
    [Header("NPC Info")]
    public string npcName = "Guard";
    [TextArea(3, 10)]
    public string[] dialogueLines = {
        "Welcome to the city, traveler.",
        "Be careful, the streets are dangerous at night.",
        "If you need supplies, visit the market by the gate."
    };

    [Header("UI References")]
    public GameObject interactPrompt;
    public GameObject dialoguePanel;
    public TMP_Text npcNameText;
    public TMP_Text dialogueTextUI;

    [Header("Typing Effect")]
    public float typingSpeed = 0.03f;

    [Header("Look Settings")]   // NEW
    public Transform player;    // Reference to the player
    public float lookSpeed = 5f;

    private bool isPlayerInRange = false;
    private bool isDialogueOpen = false;
    private bool isTyping = false;
    private int currentLineIndex = 0;
    private Coroutine typingCoroutine;

    private NavMeshAgent agent;  // NEW
    private Animator anim;       // Optional if your NPC has animations

    void Start()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        agent = GetComponent<NavMeshAgent>(); // NEW
        anim = GetComponent<Animator>();      // NEW (optional)
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform; // NEW
            isPlayerInRange = true;
            if (!isDialogueOpen && interactPrompt != null)
                interactPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            CloseDialogue();
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }
    }

    void Update()
    {
        if (!isPlayerInRange) return;

        // NEW – face player while talking
        if (isDialogueOpen && player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            if (direction.magnitude > 0.1f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookSpeed);
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isDialogueOpen)
            {
                OpenDialogue();
            }
            else if (isTyping)
            {
                FinishTypingLine();
            }
            else
            {
                NextLine();
            }
        }
    }

    void OpenDialogue()
    {
        isDialogueOpen = true;
        currentLineIndex = 0;

        if (agent != null) agent.isStopped = true; // NEW – stop walking
        if (anim != null) anim.SetBool("isWalking", false); // NEW – optional animation

        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
            npcNameText.text = npcName;
            StartTyping(dialogueLines[currentLineIndex]);
        }
    }

    void StartTyping(string text)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(text));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueTextUI.text = "";

        foreach (char c in text)
        {
            dialogueTextUI.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void FinishTypingLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueTextUI.text = dialogueLines[currentLineIndex];
        isTyping = false;
    }

    void NextLine()
    {
        currentLineIndex++;

        if (currentLineIndex < dialogueLines.Length)
        {
            StartTyping(dialogueLines[currentLineIndex]);
        }
        else
        {
            CloseDialogue();
        }
    }

    void CloseDialogue()
    {
        isDialogueOpen = false;
        isTyping = false;

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (interactPrompt != null && isPlayerInRange) interactPrompt.SetActive(true);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        if (agent != null) agent.isStopped = false; // NEW – resume walking
    }
}
