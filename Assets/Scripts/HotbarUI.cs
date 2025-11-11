using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarUI : MonoBehaviour
{
    public Inventory inventory;
    public GameObject slotPrefab;
    public Transform slotParent;
    private int hotbarSize = 5;

    private int selectedSlot = 0;
    public int SelectedSlot => selectedSlot; // readable by PlayerPickup

    void Start()
    {
        if (inventory != null)
        {
            inventory.onInventoryChangedCallback += RefreshHotbar;
            RefreshHotbar();
        }
    }

    void Update()
    {
        // Select hotbar slots using 1-5
        for (int i = 0; i < hotbarSize; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                SelectSlot(i);
        }
    }

    void SelectSlot(int index)
    {
        selectedSlot = Mathf.Clamp(index, 0, hotbarSize - 1);
        RefreshHotbar();
    }

    public void RefreshHotbar()
    {
        if (inventory == null || slotPrefab == null || slotParent == null) return;

        foreach (Transform child in slotParent)
            Destroy(child.gameObject);

        for (int i = 0; i < hotbarSize; i++)
        {
            GameObject slot = Instantiate(slotPrefab, slotParent);

            Image iconImage = slot.transform.Find("Icon")?.GetComponent<Image>();
            TMP_Text infoText = slot.transform.Find("InfoText")?.GetComponent<TMP_Text>();
            Image background = slot.GetComponent<Image>();

            if (iconImage == null || infoText == null || background == null)
                continue;

            string slotNumber = (i + 1).ToString();

            if (i < inventory.slots.Length)
            {
                var slotData = inventory.slots[i];
                if (slotData.item != null && slotData.item.itemData != null)
                {
                    iconImage.sprite = slotData.item.itemData.icon;
                    iconImage.enabled = true;
                    infoText.text = $"{slotNumber}: {slotData.item.itemData.itemName} x{slotData.item.amount}";
                }
                else
                {
                    iconImage.enabled = false;
                    infoText.text = $"{slotNumber}: Empty";
                }
            }
            else
            {
                iconImage.enabled = false;
                infoText.text = $"{slotNumber}: Empty";
            }

            // Highlight selected slot
            background.color = (i == selectedSlot) ? Color.yellow : Color.white;
        }
    }
}
