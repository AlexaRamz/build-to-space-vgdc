using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildGrid
{
    public int width;
    public int height;
    public Vector2Int bottomLeft;

    public Dictionary<Vector2Int, BuildObject> gridObjects;

    public BuildGrid(int width, int height, Vector2Int bottomLeft)
    {
        this.width = width;
        this.height = height;
        this.bottomLeft = bottomLeft;

        gridObjects = new Dictionary<Vector2Int, BuildObject>();
    }
    public BuildGrid Clone()
    {
        BuildGrid newGrid = new BuildGrid(width, height, bottomLeft);
        Dictionary<Vector2Int, BuildObject> newGridObjects = new Dictionary<Vector2Int, BuildObject>();
        foreach (KeyValuePair<Vector2Int, BuildObject> o in gridObjects)
        {
            newGridObjects[o.Key] = o.Value.Clone();
        }
        newGrid.gridObjects = newGridObjects;
        return newGrid;
    }
    Vector2Int TiletoGridPos(Vector2Int pos)
    {
        return new Vector2Int(pos.x - bottomLeft.x, pos.y - bottomLeft.y);
    }
    public bool IsWithinGrid(Vector2Int tilePos)
    {
        Vector2Int pos = TiletoGridPos(tilePos);
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }
    public BuildObject GetValue(Vector2Int tilePos)
    {
        Vector2Int pos = TiletoGridPos(tilePos);
        if (!IsWithinGrid(tilePos) || !gridObjects.ContainsKey(pos)) return null;
        return gridObjects[pos];
    }
    public bool SetValue(Vector2Int tilePos, BuildObject value)
    {
        Vector2Int pos = TiletoGridPos(tilePos);
        if (!IsWithinGrid(tilePos)) return false;
        gridObjects[pos] = value;
        return true;
    }
    public bool RemoveValue(Vector2Int tilePos)
    {
        Vector2Int pos = TiletoGridPos(tilePos);
        if (!IsWithinGrid(tilePos) || !gridObjects.ContainsKey(pos)) return false;
        gridObjects.Remove(pos);
        return true;
    }
    public bool HasAdjacentFromGridPos(Vector2Int gridPos)
    {
        Vector2Int[] adjShifts = {
            new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1)
        };
        foreach (Vector2Int shift in adjShifts)
        {
            Vector2Int adjPos = gridPos + shift;
            if (GetValue(adjPos) != null)
                return true;
        }
        return false;
    }
}
