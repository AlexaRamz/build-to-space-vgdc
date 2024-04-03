using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Category
{
    public string name;
    public Sprite image;
    public List<Build> builds = new List<Build>();

    public Build GetBuildFromIndex(int index)
    {
        if (index < 0 || index >= builds.Count) return null;
        return builds[index];
    }
}
[CreateAssetMenu(fileName = "New BuildCatalog", menuName = "Scriptable Objects/BuildCatalog")]
public class BuildCatalog : ScriptableObject
{
    public List<Category> categories = new List<Category>();
    private Category currentCategory;

    public Build GetBuild(int index)
    {
        return currentCategory.GetBuildFromIndex(index);
    }
    public Category GetCategory()
    {
        return currentCategory;
    }
    public Category SetCategory(int index)
    {
        if (index < 0 || index >= categories.Count) return null;
        currentCategory = categories[index];
        return currentCategory;
    }

    // Add category
    // Add build
}
