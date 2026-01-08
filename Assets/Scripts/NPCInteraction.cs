using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.AI;   // NEW – for stopping the NPC
using UnityEngine.UI;
using UnityEngine.Events;

public class NPCInteraction : MonoBehaviour
{
    public enum NPCType { None, Giver, Receiver }
    [Header("NPC Type")]
    public NPCType npcType = NPCType.None;

    [Header("Give Item Settings (For Giver NPC)")]
    public ItemData itemToGive;
    public int giveAmount = 1;
    public bool giveOnlyOnce = true;
    private bool hasGiven = false;

    [Header("Receiver Settings (For NPC that asks for an item)")]
    public ItemData itemRequested;
    public int requestedAmount = 1;

    [Header("Receiver UI Buttons")]
    public GameObject giveButton;
    public GameObject declineButton;

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

    [Header("Give Item Button")]
    public Button takeItemButton;   // <- ADD THIS BUTTON

    [Header("Typing Effect")]
    public float typingSpeed = 0.03f;

    [Header("Look Settings")]   // NEW
    public Transform player;    // Reference to the player
    public float lookSpeed = 5f;

    [Header("Events")]
    public UnityEvent onItemGivenToNPC;

    [Header("Boss Settings")]
    public bool isBoss = false;
    [Header("King Settings")]
    public bool isKing = false;

    [Header("Code Piece (Optional)")]
public bool givesCodeDigit = false;
[Range(0, 4)] public int codeIndex; // 0 = first digit, 4 = last digit
[Range(0, 9)] public int codeDigit;

    private bool isPlayerInRange = false;
    private bool isDialogueOpen = false;
    private bool isTyping = false;
    private int currentLineIndex = 0;
    private Coroutine typingCoroutine;

    private NavMeshAgent agent;  // NEW
    private Animator anim;       // Optional if your NPC has animations

    private Inventory playerInventory;   // <-- REAL inventory script

    [Header("References")]
    public Inventory playerInventoryManual;


    void Start()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        if (giveButton != null) giveButton.SetActive(false);
        if (declineButton != null) declineButton.SetActive(false);

        agent = GetComponent<NavMeshAgent>(); // NEW
        anim = GetComponent<Animator>();      // NEW (optional)
        playerInventory = Inventory.Instance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform; // NEW

            GameObject sm = GameObject.Find("ScriptManager");
            if (sm != null)
            {
            playerInventory = sm.GetComponent<Inventory>();
            if (playerInventory == null)
                Debug.LogError("No Inventory component found on ScriptManager!");
            }
            else
            {
                Debug.LogError("No GameObject named ScriptManager found in scene!");
            }

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

        Cursor.lockState = CursorLockMode.None;   // unlock mouse
        Cursor.visible = true;   
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
            return;
        }
        
        HandleEndOfDialogue();
    }

    void HandleEndOfDialogue()
    {
        if (isBoss)
    {
        GameProgress.Instance.talkedToBoss = true;
    }
        if(isKing)
        {
           GameProgress.Instance.talkedToKing = true; 
        }
    if (givesCodeDigit)
    {
        GameProgress.Instance.medievalCode[codeIndex] = codeDigit;
    }

        if (npcType == NPCType.Giver)
        {
            ShowGiveButton();
        }
        else if (npcType == NPCType.Receiver)
        {
            ShowReceiverOptions();
        }
        else
        {
            CloseDialogue();
        }
    }

    void ShowGiveButton()
    {
        if (giveOnlyOnce && hasGiven)
        {
            CloseDialogue();
            return;
        }

        giveButton.SetActive(true);
        giveButton.GetComponent<Button>().onClick.RemoveAllListeners();
        giveButton.GetComponent<Button>().onClick.AddListener(GiveItemToPlayer);
    }

    void GiveItemToPlayer()
    {
        playerInventory.AddItem(itemToGive, giveAmount);

        hasGiven = true;
        giveButton.SetActive(false);

        CloseDialogue();
    }
    void ShowReceiverOptions()
    {
        giveButton.SetActive(true);
        declineButton.SetActive(true);

        giveButton.GetComponent<Button>().onClick.RemoveAllListeners();
        declineButton.GetComponent<Button>().onClick.RemoveAllListeners();

        giveButton.GetComponent<Button>().onClick.AddListener(ButtonGiveItem);
        declineButton.GetComponent<Button>().onClick.AddListener(ButtonDecline);
    }

    public void ButtonGiveItem()
{
    playerInventory = Inventory.Instance;
    if (playerInventory == null)
    {
        Debug.LogError("Player inventory is NULL!");
        return;
    }

    if (playerInventory.HasItem(itemRequested, requestedAmount))
    {
        playerInventory.RemoveItem(itemRequested, requestedAmount);
        Debug.Log("Item given to NPC!");

        onItemGivenToNPC?.Invoke();
    }
    else
    {
        Debug.Log("Player does NOT have the item!");
    }

    giveButton.SetActive(false);
    declineButton.SetActive(false);
    CloseDialogue();
}

    bool PlayerHasItem()
    {
        int total = 0;

        foreach (var slot in playerInventory.slots)
        {
            if (slot.item != null && slot.item.itemData == itemRequested)
            {
                total += slot.item.amount;
                if (total >= requestedAmount)
                    return true;
            }
        }
        return false;
    }

    void RemoveRequestedItem()
    {
        int amountToRemove = requestedAmount;

        for (int i = 0; i < playerInventory.slots.Length; i++)
        {
            var slot = playerInventory.slots[i].item;
            if (slot != null && slot.itemData == itemRequested)
            {
                int take = Mathf.Min(slot.amount, amountToRemove);
                slot.amount -= take;
                amountToRemove -= take;

                if (slot.amount <= 0)
                    playerInventory.slots[i].item = null;

                if (amountToRemove <= 0)
                    break;
            }
        }
        
        playerInventory.AddItem(itemToGive, giveAmount);
    }

    public void ButtonDecline()
    {
        giveButton.SetActive(false);
        declineButton.SetActive(false);

        CloseDialogue();
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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;  
    }
}
