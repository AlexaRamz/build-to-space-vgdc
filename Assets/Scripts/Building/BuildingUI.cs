using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
    public Transform categoryContainer, buildContainer, materialDisplayContainer;
    public GameObject categoryButtonTemplate, buildButtonTemplate, materialDisplayTemplate;
    public TMPro.TextMeshProUGUI buildName, buildDescription;

    BuildingSystem buildSys;
    [SerializeField] private InventoryManager plrInv;

    private void Start()
    {
        buildSys = BuildingSystem.Instance;
    }
    private void OnEnable()
    {
        buildSys = BuildingSystem.Instance;
        SetCatalog(buildSys.buildCatalog);
        buildSys.StartBuilding();
        plrInv.updateUIEvent += UpdateMaterials;
    }
    private void OnDisable()
    {
        buildSys.EndBuilding();
        plrInv.updateUIEvent -= UpdateMaterials;
    }
    public void SetCatalog(BuildCatalog buildCatalog)
    {
        ClearCategories();
        for (int i = 0; i < buildCatalog.categories.Count; i++)
        {
            int index = i;
            GameObject button = Instantiate(categoryButtonTemplate, categoryContainer);
            button.transform.Find("Image").GetComponent<Image>().sprite = buildCatalog.categories[i].image;
            button.GetComponent<Button>().onClick.AddListener(delegate { ChangeCategory(index); });
        }
        if (buildCatalog.categories.Count > 0)
            ChangeCategory(0);
    }
    void ClearCategories()
    {
        foreach (Transform c in categoryContainer)
        {
            Destroy(c.gameObject);
        }
    }

    public void ChangeBuild(int i)
    {
        SetBuildSelection(i);
        Build build = buildSys.SetBuildObject(i);
        SetInfo(build);
    }
    public void ChangeCategory(int i)
    {
        SetCategorySelection(i);
        BuildCategory category = buildSys.SetCategory(i);
        SetBuilds(category);
    }
    public void UpdateMaterials()
    {
        Build build = buildSys.currentBuildObject.build;
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
        if (build == null) return;

        ClearInfo();
        buildName.text = build.name;
        buildName.color = new Color32(255, 255, 255, 255);
        buildDescription.text = build.description;
        foreach (ItemAmountInfo m in build.materials)
        {
            Transform display = Instantiate(materialDisplayTemplate, materialDisplayContainer).transform;
            Image image = display.Find("Image").GetComponent<Image>();
            image.sprite = m.item.image;
            Text text = display.Find("Amount").GetComponent<Text>();
            text.text = m.amount.ToString();
            if (plrInv.GetItemAmount(m.item) < m.amount)
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
    void SetCategorySelection(int i)
    {
        foreach (Transform c in categoryContainer)
        {
            c.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
        }
        categoryContainer.GetChild(i).GetComponent<Image>().color = new Color32(255, 255, 255, 60);
    }
    void SetBuildSelection(int i)
    {
        foreach (Transform child in buildContainer)
        {
            child.GetComponent<SelectionButton>().SetSelection(false);
        }
        buildContainer.GetChild(i).GetComponent<SelectionButton>().SetSelection(true);
    }
    void ClearBuilds()
    {
        foreach (Transform c in buildContainer)
        {
            Destroy(c.gameObject);
        }
    }
    void SetBuilds(BuildCategory category)
    {
        if (category == null) return;

        ClearBuilds();
        List<Build> buildList = category.builds;
        for (int i = 0; i < buildList.Count; i++)
        {
            int index = i;
            GameObject button = Instantiate(buildButtonTemplate, buildContainer);
            button.GetComponent<SelectionButton>().image.sprite = buildList[i].rotations[0].GetSprite();
            button.GetComponent<Button>().onClick.AddListener(delegate { ChangeBuild(index); });
        }
        SetInfo(category.name);
    }
}