using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildObject
{
    public Build build;
    public int rot = 0;
    public GameObject gridObject;

    public BuildObject(Build build, int rot=0, GameObject gridObject=null)
    {
        this.build = build;
        this.rot = rot;
        this.gridObject = gridObject;
    }
    public BuildObject Clone()
    {
        return new BuildObject(build, rot, gridObject);
    }
    public Rotation GetRotation()
    {
        if (build == null || build.rotations == null || build.rotations.Length <= 0)
        {
            Debug.Log("No rotations saved");
            return null;
        }
        return build.rotations[rot];
    }
    public void AdvanceRotation()
    {
        if (build == null || build.rotations == null) return;
        rot = (rot + 1) % build.rotations.Length;
    }
}
