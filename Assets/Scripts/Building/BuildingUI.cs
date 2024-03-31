using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildingUI : MonoBehaviour
{
    public BuildingSystem buildSys;
    public Transform categoryContainer, buildContainer, materialDisplayContainer;
    public GameObject categoryButtonTemplate, buildButtonTemplate, materialDisplayTemplate;
    public TMPro.TextMeshProUGUI buildName, buildDescription;

    private Inventory plr;
    private Canvas canvas;
    private MenuManager menuManager;

    void Awake()
    {
        plr = GameObject.Find("Player").GetComponent<Inventory>();
        canvas = GetComponent<Canvas>();
        menuManager = GameObject.Find("MenuManager").GetComponent<MenuManager>();
    }
    public void StartBuilding()
    {

    }
    public void EndBuilding()
    {

    }
    public void OpenMenu() // Called only by menu manager
    {
        canvas.enabled = true;
    }
    public void CloseMenu() // Called only by menu manager
    {
        canvas.enabled = false;
        ///These don't need to be called when the menu is called. I'm pretty sure it doesn't accomplish anything.
        ///ClearInfo();
        ///ClearBuilds();
        buildSys.EndBuilding();
    }
    public void ChangeBuild(int i)
    {
        SetBuildSelection(i);
        Build build = buildSys.GetBuild(i);
        buildSys.StartPlacing(build);
        SetInfo(build);
    }
    public void ChangeCategory(int i)
    {
        SetCategorySelection(i);
        buildSys.ChangeCategory(i);
    }
    public void UpdateMaterials()
    {
        Build build = buildSys.GetBuild();
        if (build != null)
        {
            SetInfo(build);
        }
    }
    void ClearInfo()
    {
        foreach (Transform c in materialDisplayContainer)
        {
            Destroy(c.gameObject);
        }
    }
    void SetInfo(Build build)
    {
        ClearInfo();
        buildName.text = build.name;
        buildName.color = new Color32(255, 255, 255, 255);
        buildDescription.text = build.description;
        foreach (ResourceAmount m in build.materials)
        {
            Transform display = Instantiate(materialDisplayTemplate, materialDisplayContainer).transform;
            Image image = display.Find("Image").GetComponent<Image>();
            image.sprite = plr.GetResourceImage(m.resource);
            Text text = display.Find("Amount").GetComponent<Text>();
            text.text = m.amount.ToString();
            if (plr.GetResourceAmount(m.resource) < m.amount)
            {
                image.color = text.color = new Color32(165, 165, 165, 255);
            }
            else
            {
                image.color = text.color = new Color32(255, 255, 255, 255);
            }
        }
    }
    void SetInfo(string categoryName)
    {
        ClearInfo();
        buildName.text = "(" + categoryName + ")";
        buildName.color = new Color32(165, 165, 165, 255);
        buildDescription.text = "";
    }
    public void SetCategorySelection(int i)
    {
        foreach (Transform c in categoryContainer)
        {
            c.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
        }
        categoryContainer.GetChild(i).GetComponent<Image>().color = new Color32(255, 255, 255, 60);
    } 
    public void SetCategories(List<Category> categories)
    {
        for (int i = 0; i < categories.Count; i++)
        {
            int index = i;
            GameObject button = Instantiate(categoryButtonTemplate, categoryContainer);
            button.transform.Find("Image").GetComponent<Image>().sprite = categories[i].image;
            button.GetComponent<Button>().onClick.AddListener(delegate { ChangeCategory(index); });
        }
    }
    public void SetBuildSelection(int i)
    {
        foreach (Transform child in buildContainer)
        {
            child.GetComponent<SelectionButton>().SetSelection(false);
        }
        buildContainer.GetChild(i).GetComponent<SelectionButton>().SetSelection(true);
    }
    public void ClearBuilds()
    {
        foreach (Transform c in buildContainer)
        {
            Destroy(c.gameObject);
        }
    }
    public void SetBuilds(Category category)
    {
        ClearBuilds();
        List<Build> buildList = category.builds;
        for (int i = 0; i < buildList.Count; i++)
        {
            int index = i;
            GameObject button = Instantiate(buildButtonTemplate, buildContainer);
            button.GetComponent<SelectionButton>().SetImage(buildList[i].rotations[0].GetSprite());
            button.GetComponent<Button>().onClick.AddListener(delegate { ChangeBuild(index); });
        }
        SetInfo(category.name);
    }
    public bool IsOnUI()
    {
        GraphicRaycaster gr = GetComponent<GraphicRaycaster>();
        PointerEventData ped = new PointerEventData(null);
        ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(ped, results);
        return results.Count != 0;
    }
}
