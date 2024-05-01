using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemButtonTemplate;
    [SerializeField] Transform buttonContainer;
    [SerializeField] private InventoryManager plrInv;
    List<Button> buttons = new List<Button>();

    public void Start()
    {
        DisplayItems(plrInv.items);
    }

    public void SelectItem(int index)
    {
        
    }
    void ClearItems()
    {
        foreach (Transform c in buttonContainer)
        {
            if (c.name != "CancelButton")
            {
                Destroy(c.gameObject);
            }
        }
    }
    void DisplayItems(List<ItemAmountInfo> items)
    {
        ClearItems();
        for (int i = 0; i < items.Count; i++)
        {
            int index = i;
            GameObject button = Instantiate(itemButtonTemplate, buttonContainer);
            SelectionButton selectButton = button.GetComponent<SelectionButton>();
            selectButton.image.sprite = items[i].item.image;
            selectButton.text.text = items[i].amount.ToString();
            button.GetComponent<Button>().onClick.AddListener(delegate { SelectItem(index); });
        }
    }
}
