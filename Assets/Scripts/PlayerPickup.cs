using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public Transform holdPoint;
    public float pickupRange = 3f;
    public LayerMask pickupLayer;
    public KeyCode pickupKey = KeyCode.E;        // pick up / drop
    public KeyCode addToInventoryKey = KeyCode.F;
    public KeyCode dropInventoryKey = KeyCode.G;

    public HotbarUI hotbarUI; // assign in inspector

    private GameObject heldObject;
    private Inventory inventory;

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        if (inventory == null)
            Debug.LogError("No Inventory found in scene!");
    }

    void Update()
    {
        // Toggle pickup / drop
        if (Input.GetKeyDown(pickupKey))
        {
            if (heldObject == null)
                TryPickup();
            else
                DropHeldObject();
        }

        // Add held object to inventory
        if (Input.GetKeyDown(addToInventoryKey) && heldObject != null)
            AddHeldObjectToInventory();

        // Drop selected hotbar item
        if (Input.GetKeyDown(dropInventoryKey))
            DropSelectedInventoryItem();

        // Keep held object in front of player
        if (heldObject != null)
            heldObject.transform.position = holdPoint.position;
    }

    void TryPickup()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickupLayer))
        {
            GameObject obj = hit.collider.gameObject;

            if (obj.TryGetComponent(out PickupItem pickup))
            {
                HoldObject(obj);
            }
        }
    }

    void HoldObject(GameObject obj)
    {
        if (heldObject != null)
            DropHeldObject();

        heldObject = obj;

        if (heldObject.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        heldObject.transform.SetParent(holdPoint);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;
    }

    void AddHeldObjectToInventory()
    {
        if (heldObject == null) return;

        if (heldObject.TryGetComponent(out PickupItem pickup))
        {
            inventory.AddItem(pickup.itemData, 1);

            if (heldObject.TryGetComponent(out WorldItem worldItem))
            {
                SaveSystem.RegisterCollectedItem(worldItem.uniqueID);
                Debug.Log($"🗂 Registered collected item: {worldItem.uniqueID}");
            }
        }

        Destroy(heldObject);
        heldObject = null;
    }

    void DropHeldObject()
    {
        if (heldObject == null) return;

        heldObject.transform.SetParent(null);

        if (heldObject.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            // optional: small forward push when dropping
            rb.AddForce(Camera.main.transform.forward * 2f, ForceMode.Impulse);
        }

        heldObject = null;
    }

    void DropSelectedInventoryItem()
    {
        if (inventory == null || hotbarUI == null) return;

        int slot = hotbarUI.SelectedSlot;
        inventory.DropItem(slot, holdPoint);
    }
}
