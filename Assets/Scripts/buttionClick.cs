using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum ButtonMode
{
    InputOnly,
    PressurePlate,
    Both
}

public class buttionClick : MonoBehaviour
{
    [Header("Mode")]
    public ButtonMode mode = ButtonMode.Both;
    public bool staysDown = false;

    [Header("Detection")]
    public float detectionRange = 2f;                      // fallback distance check
    public Vector3 overlapSize = new Vector3(1.5f, 1.0f, 1.5f);
    public Vector3 overlapCenterOffset = Vector3.zero;    // local offset for overlap center
    public LayerMask overlapMask = ~0;                    // default = everything
    public bool useOverlapBox = true;
    public string[] pressureTags = new string[] { "Player", "Pickup" };

    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Visual")]
    public Transform buttonVisual;                         // assign child top that moves
    public Vector3 pressedOffset = new Vector3(0, -1f, 0);
    public float pressSpeed = 25f;
    public float resetDelay = 0.6f;

    [Header("UI")]
    public GameObject promptUI;                            // assign your Text UI here (optional)

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip activateSound;
    public AudioClip deactivateSound;

    [Header("Events")]
    public UnityEvent onActivate;
    public UnityEvent onDeactivate;

    // internal
    Vector3 initialPos;
    bool isPressed = false;
    bool playerNearby = false;
    bool ObjectsNearby = false;
    bool activatedByInput = false;
    Transform foundPlayer = null;



    void Start()
    {
        if (buttonVisual == null) buttonVisual = transform;
        initialPos = buttonVisual.localPosition;

        // try to find player by tag first, then CharacterController
        var p = GameObject.FindWithTag("Player");
        if (p != null) foundPlayer = p.transform;
        else
        {
            var cc = FindObjectOfType<CharacterController>();
            if (cc != null) foundPlayer = cc.transform;
        }

        // if no prompt assigned, create a simple screen-space prompt automatically
        if (promptUI == null)
            CreateDefaultPrompt();

        if (promptUI != null)
            promptUI.SetActive(false);

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        playerNearby = CheckObjectsNearby();

        // toggle prompt (only when Input is allowed and not pressed yet)
        if (promptUI != null)
            promptUI.SetActive(playerNearby && !isPressed && (mode == ButtonMode.InputOnly || mode == ButtonMode.Both));

        // handle modes
        switch (mode)
        {
            case ButtonMode.InputOnly:
                if (playerNearby && Input.GetKeyDown(interactKey))
                    ActivateButton(true);
                break;

            case ButtonMode.PressurePlate:
                if (playerNearby && !isPressed)
                    ActivateButton(false);

                if (!playerNearby && isPressed && !staysDown)
                    ResetButton(); // auto reset when leaving
                break;

            case ButtonMode.Both:
                if (playerNearby && Input.GetKeyDown(interactKey) && !isPressed)
                    ActivateButton(true);
                if (playerNearby && !isPressed)
                    ActivateButton(false); // pressure plate fallback
                if (!playerNearby && isPressed && !staysDown && (activatedByInput == false))
                    ResetButton();
                break;
        }

        // animate visual press
        Vector3 targetPos = isPressed ? initialPos + pressedOffset : initialPos;
        buttonVisual.localPosition = Vector3.Lerp(buttonVisual.localPosition, targetPos, Time.deltaTime * pressSpeed);
    }


    bool CheckObjectsNearby()
    {
        // 1) OverlapBox approach (works regardless of Rigidbody on player)
        if (useOverlapBox)
        {
            Vector3 worldCenter = transform.position + transform.TransformDirection(overlapCenterOffset);
            Collider[] hits = Physics.OverlapBox(worldCenter, overlapSize * 0.5f, transform.rotation, overlapMask);
            foreach (var c in hits)
            {
                if (c == null) continue;
                foreach (var tag in pressureTags)
                {
                    if (c.CompareTag(tag))
                        return true;
                }
                //if (c == null) continue;
                //if (c.CompareTag("Player") || c.GetComponent<CharacterController>() != null || c.CompareTag("Pickup"))
                //return true;
            }
        }

        // 2) If we found a player transform earlier, do a distance fallback
        if (foundPlayer != null)
        {
            if (Vector3.Distance(foundPlayer.position, transform.position) <= detectionRange)
                return true;
        }

        // 3) Last resort: find any CharacterController and check distance (works for some asset controllers)
        var cc = FindObjectOfType<CharacterController>();
        if (cc != null)
        {
            if (Vector3.Distance(cc.transform.position, transform.position) <= detectionRange)
                return true;
        }

        return false;
    }

    bool IsAnyObjectOnPlate()
    {
        Vector3 worldCenter = transform.position + transform.TransformDirection(overlapCenterOffset);
        Collider[] hits = Physics.OverlapBox(worldCenter, overlapSize * 0.5f, transform.rotation, overlapMask);

        foreach (var c in hits)
        {
            if (c == null) continue;

            foreach (var tag in pressureTags)
            {
                if (c.CompareTag(tag))
                    return true; // at least one valid object is on plate
            }
        }

        return false; // nothing on plate
    }

    void ActivateButton(bool byInput)
    {

        if (isPressed) return;

        // cancel any pending Reset to avoid double invocation
        CancelInvoke(nameof(ResetButton));

        isPressed = true;
        activatedByInput = byInput;
        Debug.Log($"DualButtonRobust_v2: Activated (byInput={activatedByInput})");
        onActivate?.Invoke();

        // auto-reset logic:
        if (staysDown)
        {
            // do nothing — remains down until manual reset
            return;
        }

        // If activated by input, use resetDelay
        if (activatedByInput)
        {
            Invoke(nameof(ResetButton), resetDelay);
        }
    }


    void ResetButton()
    {
        CancelInvoke(nameof(ResetButton));
        isPressed = false;
        activatedByInput = false;
        Debug.Log("DualButtonRobust_v2: Reset");

        onDeactivate?.Invoke();
    }

    // Create a simple on-screen prompt so you can see it without manual UI setup
    void CreateDefaultPrompt()
    {
        var canvasGO = new GameObject("DualButtonCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        var txtGO = new GameObject("PromptText");
        txtGO.transform.SetParent(canvasGO.transform);
        var txt = txtGO.AddComponent<Text>();
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.text = "Press E to Activate";
        txt.alignment = TextAnchor.MiddleCenter;
        txt.fontSize = 34;
        txt.raycastTarget = false;

        var rt = txtGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.anchoredPosition = new Vector2(0, 120);
        rt.sizeDelta = new Vector2(700, 80);

        promptUI = txtGO;
    }

    // visualize overlap in scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Matrix4x4 trs = Matrix4x4.TRS(transform.position + transform.TransformDirection(overlapCenterOffset), transform.rotation, Vector3.one);
        Gizmos.matrix = trs;
        Gizmos.DrawWireCube(Vector3.zero, overlapSize);
    }
    
    public bool IsPressed()
    {
        return isPressed;
    }


}
