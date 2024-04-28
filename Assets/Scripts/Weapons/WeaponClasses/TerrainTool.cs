using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTool : Tool
{
    void Mine()
    {
        TerrainManager.Instance.DeleteAtMouse();
    }

    public override bool Use()
    {
        if (!base.Use()) return false;

        Mine();
        return true;
    }
}
