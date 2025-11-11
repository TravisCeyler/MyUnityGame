using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public GameObject slotPrefab;
    public Transform slotParent;

    void Start()
    {
        inventory.onInventoryChangedCallback += RefreshUI;
        RefreshUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            gameObject.SetActive(!gameObject.activeSelf);
    }

    void RefreshUI()
    {
        foreach (Transform child in slotParent)
            Destroy(child.gameObject);

       // foreach (var item in inventory.items)
        {
            GameObject slot = Instantiate(slotPrefab, slotParent);
           // slot.transform.Find("Icon").GetComponent<Image>().sprite = item.icon;
            slot.transform.Find("Icon").GetComponent<Image>().enabled = true;
           // slot.transform.Find("Amount").GetComponent<TMP_Text>().text = item.amount.ToString();
        }
    }
}
