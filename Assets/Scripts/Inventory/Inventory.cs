using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<ResourceInfo> resources = new List<ResourceInfo>();

    private void Start()
    {
        plrMove = GetComponent<PlayerMovement>();
        buildSys = FindObjectOfType<BuildingSystem>();
        invUI.plrInv = this;
    }
    // Inventory managing
    public void Collect(ResourceType thisResource)
    {
        AddAmount(thisResource, 1);
        UpdateDisplays();
    }
    void AddAmount(ResourceType thisResource, int thisAmount)
    {
        if ((int)thisResource < resources.Count)
        {
            resources[(int)thisResource].amount += thisAmount;
            return;
        }
        Debug.Log("Resource info not available");
    }
    bool CheckResource(ResourceType thisResource, int amount)
    {
        return amount <= GetResourceAmount(thisResource);
    }
    public bool CheckMaterials(List<ResourceAmount> requiredMats)
    {
        foreach (ResourceAmount detail in requiredMats)
        {
            if (!CheckResource(detail.resource, detail.amount))
            {
                return false;
            }
        }
        return true;
    }
    public void AddMaterials(List<ResourceAmount> requiredMats)
    {
        foreach (ResourceAmount detail in requiredMats)
        {
            AddAmount(detail.resource, detail.amount);
        }
        UpdateDisplays();
    }
    public void DepleteMaterials(List<ResourceAmount> requiredMats)
    {
        foreach (ResourceAmount detail in requiredMats)
        {
            if (CheckResource(detail.resource, detail.amount))
            {
                AddAmount(detail.resource, -detail.amount);
            }
        }
        UpdateDisplays();
    }
    public int GetResourceAmount(ResourceType thisResource)
    {
        if ((int)thisResource < resources.Count)
        {
            return resources[(int)thisResource].amount;
        }
        Debug.Log("Resource info not available");
        return 0;
    }
    public Sprite GetResourceImage(ResourceType thisResource)
    {
        if ((int)thisResource < resources.Count)
        {
            return resources[(int)thisResource].resource.image;
        }
        Debug.Log("Resource info not available");
        return null;
    }

    // UI
    BuildingSystem buildSys;
    public InventoryUI invUI;
    void UpdateDisplays()
    {
        invUI.SetDisplays();
        UpdateHoldingDisplay();
        buildSys.UpdateMaterials();
    }

    // Holding
    public SpriteRenderer holdOrigin;
    PlayerMovement plrMove;

    public ResourceDisplay holdDisplay;
    [HideInInspector] public int currentResourceIndex = -1;
    ResourceInfo GetHolding()
    {
        if (currentResourceIndex != -1)
        {
            return resources[currentResourceIndex];
        }
        return null;
    }
    void UpdateHoldingDisplay()
    {
        ResourceInfo holding = GetHolding();
        if (holding != null)
        {
            holdDisplay.SetResourceSimple(holding);
            invUI.InterfaceOn();
        }
        else
        {
            invUI.InterfaceOff();
        }
        UpdateHold();
    }
    void UpdateHold()
    {
        ResourceInfo holding = GetHolding();
        if (holding != null)
        {
            holdOrigin.sprite = GetHolding().resource.image;
            if (holding.amount > 0f)
            {
                holdOrigin.color = new Color32(255, 255, 255, 255);
            }
            else
            {
                holdOrigin.color = new Color32(255, 255, 255, 128);
            }
            plrMove.HoldAnim();
        }
        else
        {
            holdOrigin.sprite = null;
            plrMove.CancelHoldAnim();
        }
    }
    public void Hold(int i)
    {
        currentResourceIndex = i;
        UpdateHoldingDisplay();
    }

    // Dropping and Throwing
    public GameObject dropTemplate;
    IEnumerator currentDelay;
    void Drop()
    {
        ResourceInfo holding = GetHolding();
        if (holding != null && holding.amount > 0)
        {
            GameObject obj = Instantiate(dropTemplate, holdOrigin.transform.position, Quaternion.identity);
            obj.GetComponent<Collectable>().SetItem(holding.resource);
            resources[currentResourceIndex].amount--;
            UpdateDisplays();
            Debug.Log("new");

            holdOrigin.color = new Color32(255, 255, 255, 128);
            if (currentDelay != null)
            {
                StopCoroutine(currentDelay);
            }
            currentDelay = DropDelay();
            StartCoroutine(currentDelay);
        }
    }
    IEnumerator DropDelay()
    {
        yield return new WaitForSeconds(0.5f);
        UpdateHold();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            Drop();
        }
    }
}
