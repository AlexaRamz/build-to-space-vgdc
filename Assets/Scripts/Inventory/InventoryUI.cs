using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemButtonTemplate;
    [SerializeField] Transform buttonContainer;
    [SerializeField] private InventoryManager plrInv;
    List<SelectionButton> buttons = new List<SelectionButton>();
    int currentIndex;
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    public void OnEnable()
    {
        currentIndex = -1;
        plrInv.inventoryModifiedEvent += UpdateAll;
        UpdateAll();
    }
    public void OnDisable()
    {
        plrInv.inventoryModifiedEvent -= UpdateAll;
    }
    void UpdateAll()
    {
        DisplayItems(plrInv.items);
    }
    public void ToggleItem(int index)
    {
        if (index != currentIndex)
        {
            SelectItem(index);
        }
        else
        {
            DeselectItem();
        }
    }
    public void SelectItem(int index)
    {
        if (currentIndex != -1)
        {
            buttons[currentIndex].SetSelection(false);
        }
        currentIndex = index;
        plrInv.SelectItem(currentIndex);
        buttons[currentIndex].SetSelection(true);

        ItemAmountInfo itemInfo = plrInv.GetItemFromIndex(index);
        nameText.text = itemInfo.item.name;
        descriptionText.text = itemInfo.item.description;
    }
    public void DeselectItem()
    {
        if (currentIndex != -1)
        {
            buttons[currentIndex].SetSelection(false);
            plrInv.DeselectItem();
            currentIndex = -1;
            nameText.text = descriptionText.text = "";
        }
    }
    void ClearItems()
    {
        buttons.Clear();
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
        nameText.text = descriptionText.text = "";
        for (int i = 0; i < items.Count; i++)
        {
            int index = i;
            GameObject button = Instantiate(itemButtonTemplate, buttonContainer);
            SelectionButton selectButton = button.GetComponent<SelectionButton>();
            selectButton.image.sprite = items[i].item.image;
            selectButton.text.text = items[i].amount.ToString();
            button.GetComponent<Button>().onClick.AddListener(delegate { ToggleItem(index); });
            buttons.Add(selectButton);
            if (plrInv.currentItem == items[i].item)
            {
                selectButton.SetSelection(true);
                currentIndex = i;
            }
        }
    }
}
