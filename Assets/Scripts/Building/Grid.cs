using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int width;
    private int height;
    private Vector2Int bottomLeft;

    Dictionary<Vector2Int, BuildObject> gridObjects;

    public Grid(int width, int height, Vector2Int bottomLeft)
    {
        this.width = width;
        this.height = height;
        this.bottomLeft = bottomLeft;

        gridObjects = new Dictionary<Vector2Int, BuildObject>();
    }
    Vector2Int TiletoGridPos(Vector2Int pos)
    {
        return new Vector2Int(pos.x - bottomLeft.x, pos.y - bottomLeft.y);
    }
    public bool IsWithinGrid(Vector2Int tilePos)
    {
        Vector2Int pos = TiletoGridPos(tilePos);
        return pos.x >= 0 && pos.x < width && pos.y >= 0 || pos.y < height;
    }
    public BuildObject GetValue(Vector2Int tilePos)
    {
        Vector2Int pos = TiletoGridPos(tilePos);
        if (!IsWithinGrid(pos) || !gridObjects.ContainsKey(pos)) return null;
        return gridObjects[pos];
    }
    public bool SetValue(Vector2Int tilePos, BuildObject value)
    {
        Vector2Int pos = TiletoGridPos(tilePos);
        if (!IsWithinGrid(pos)) return false;
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
}
