using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSystem : MonoBehaviour
{
    [SerializeField] private List<ShopItem> shopItems = new List<ShopItem>();
    Inventory plrInv;
    public GameObject shopTemplate;
    public Transform shopContainer;
    ShopItem selectedItem;

    public Image displayImage;
    public TMP_Text descriptionText;
    public TMP_Text priceText;

    private void Start()
    {
        plrInv = GameObject.Find("Player").GetComponent<Inventory>();
        SetObjects();
    }
    public void Purchase()
    {
        if (selectedItem != null)
        {
            if (plrInv.money >= selectedItem.cost)
            {
                plrInv.money -= selectedItem.cost;
                plrInv.ApplyShopItem(selectedItem);
                Debug.Log("buy");
            }
        }
    }
    public void SelectItem(int btnNo)
    {
        selectedItem = shopItems[btnNo];
        displayImage.sprite = selectedItem.image;
        descriptionText.text = selectedItem.description;
        priceText.text = selectedItem.cost.ToString();
    }
    void ClearObjects()
    {
        foreach (Transform child in shopContainer)
        {
            Destroy(child.gameObject);
        }
    }
    void SetObjects()
    {
        ClearObjects();
        for (int i = 0; i < shopItems.Count; i++)
        {
            int btnNo = i;
            GameObject button = Instantiate(shopTemplate, shopContainer);
            button.GetComponent<Button>().onClick.AddListener(delegate { SelectItem(btnNo); });
            ShopTemplate template = button.GetComponent<ShopTemplate>();
            template.text.text = shopItems[i].name;
            template.image.sprite = shopItems[i].image;
        }
    }
}
