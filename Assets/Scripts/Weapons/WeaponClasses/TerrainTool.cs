using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTool : Tool
{
    PlayerMovement plrMove;

    void Start()
    {
        plrMove = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

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

    void Update()
    {
        plrMove.GetComponent<PlayerMovement>().UpdateToolFlipX();
    }
}
