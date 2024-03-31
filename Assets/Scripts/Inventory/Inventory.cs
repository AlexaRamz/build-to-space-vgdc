using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<ResourceInfo> resources = new List<ResourceInfo>();

    public List<ToolData> tools = new List<ToolData>();
    ITool currentTool;

    public int money;

    PlayerInteraction plrInt;

    private void Start()
    {
        plrMove = GetComponent<PlayerMovement>();
        buildSys = FindObjectOfType<BuildingSystem>();
        invUI.plrInv = this;
        plrInt = GetComponent<PlayerInteraction>();

        tools.Add((ToolData)Resources.Load("Tools/Weapons/Laser Gun"));
        tools.Add((ToolData)Resources.Load("Tools/Weapons/Laser Cannon"));
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
    public Transform holdOrigin;
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
        SpriteRenderer sr = holdOrigin.GetComponent<SpriteRenderer>();
        if (holding != null)
        {
            sr.sprite = GetHolding().resource.image;
            if (holding.amount > 0f)
            {
                sr.color = new Color32(255, 255, 255, 255);
            }
            else
            {
                sr.color = new Color32(255, 255, 255, 128);
            }
            plrMove.HoldAnim();
        }
        else
        {
            sr.sprite = null;
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

            holdOrigin.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 128);
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

        /* TOOLS-WEAPONS */
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Equip(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Equip(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Equip(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Equip(3);
        }

        if (plrInt.canInteract && currentTool != null && Input.GetMouseButton(0))
        {
            currentTool.Use();
        }
    }

    /* TOOLS-WEAPONS */
    public void Equip(int index)
    {
        ClearEquip();
        if (index >= 0 && index < tools.Count && tools[index] != null)
        {
            ToolData data = tools[index];
            //Debug.Log(data.Name);
            if (InitializeTool(data, holdOrigin))
            {
                plrMove.HoldAnim();
            }
            else
            {
                Debug.Log("Failed to equip!");
            }
        }
        else
        {
            plrMove.CancelHoldAnim();
        }
    }
    void ClearEquip()
    {
        foreach (Transform child in holdOrigin.transform)
        {
            Destroy(child.gameObject);
        }
        currentTool = null;
    }
    bool InitializeTool(ToolData data, Transform origin)
    {
        if (data.prefab != null)
        {
            GameObject obj = Instantiate(data.prefab, origin.position, Quaternion.identity);
            obj.transform.parent = origin;
            obj.name = data.name;
            obj.transform.localScale = new Vector3(1, 1, 1);
            ITool tool = obj.GetComponent<ITool>();
            if (tool != null) 
            {
                tool.data = data;
                currentTool = tool;
            }
            return true;
        }
        return false;
    }
    void AddTool(ToolData tool)
    {
        tools.Add(tool);
    }

    /* SHOP ITEMS */
    public void ApplyShopItem(ShopItem item)
    {
        Debug.Log(item.GetType());
        if (item is ToolData)
        {
            AddTool((ToolData)item);
        }
        // Future: powerups
    }
}
