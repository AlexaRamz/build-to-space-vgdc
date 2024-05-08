using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildCategory
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
    public List<BuildCategory> categories = new List<BuildCategory>();
    private BuildCategory currentCategory;

    public Build GetBuild(int index)
    {
        return currentCategory.GetBuildFromIndex(index);
    }
    public BuildCategory GetCategory()
    {
        return currentCategory;
    }
    public BuildCategory SetCategory(int index)
    {
        if (index < 0 || index >= categories.Count) return null;
        currentCategory = categories[index];
        return currentCategory;
    }

    // Add category
    // Add build
}
