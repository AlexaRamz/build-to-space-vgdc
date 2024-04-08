using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildGrid
{
    public int width;
    public int height;
    public Vector2Int bottomLeft;

    public Dictionary<Vector2Int, BuildObject> gridObjects;

    public BuildGrid(Vector2Int bottomLeft, int width = int.MaxValue, int height = int.MaxValue)
    {
        this.width = width;
        this.height = height;
        this.bottomLeft = bottomLeft;

        gridObjects = new Dictionary<Vector2Int, BuildObject>();
    }
    public BuildGrid Clone(bool withGameObjects)
    {
        BuildGrid newGrid = new BuildGrid(bottomLeft, width, height);
        Dictionary<Vector2Int, BuildObject> newGridObjects = new Dictionary<Vector2Int, BuildObject>();
        foreach (KeyValuePair<Vector2Int, BuildObject> o in gridObjects)
        {
            newGridObjects[o.Key] = o.Value.Clone();
            if (!withGameObjects)
            {
                newGridObjects[o.Key].gridObject = null;
            }
        }
        newGrid.gridObjects = newGridObjects;
        return newGrid;
    }
    public static Vector3 TileToWorldPos(Vector2Int pos)
    {
        return new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0);
    }
    public Vector2Int TiletoGridPos(Vector2Int pos)
    {
        return new Vector2Int(pos.x - bottomLeft.x, pos.y - bottomLeft.y);
    }
    public Vector2Int GridtoTilePos(Vector2Int pos)
    {
        return new Vector2Int(pos.x + bottomLeft.x, pos.y + bottomLeft.y);
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

    public Vector2Int ClampBounds()
    {
        if (gridObjects.Count == 0)
        {
            width = height = 0;
            return Vector2Int.zero;
        }

        int newWidth = 0;
        int newHeight = 0;
        Vector2Int offset = new Vector2Int(int.MaxValue, int.MaxValue);
        foreach (KeyValuePair<Vector2Int, BuildObject> p in gridObjects)
        {
            if (p.Key.x < offset.x)
            {
                offset.x = p.Key.x;
            }
            if (p.Key.y < offset.y)
            {
                offset.y = p.Key.y;
            }
            if (p.Key.x + 1 > newWidth)
            {
                newWidth = p.Key.x + 1;
            }
            if (p.Key.y + 1 > newHeight)
            {
                newHeight = p.Key.y + 1;
            }
        }
        width = newWidth;
        height = newHeight;

        if (offset == Vector2Int.zero) return offset;

        bottomLeft += offset;
        Dictionary<Vector2Int, BuildObject> newGridObjects = new Dictionary<Vector2Int, BuildObject>();
        foreach (KeyValuePair<Vector2Int, BuildObject> p in gridObjects)
        {
            newGridObjects[p.Key - offset] = p.Value;
        }
        gridObjects = newGridObjects;
        return offset;
    }

    /// <summary>
    /// Increases the size of the ship. Negative numbers grow the ship to the left/bottom. Positive numbers grow the ship to the right/top
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    public void AddSize(int i = 0, int j = 0)
    {
        if (i == 0 && j == 0) //Don't do anything weird if we are not growing at all
            return;
        int offsetX = 0;
        int offsetY = 0;
        offsetX = Mathf.Abs(i);
        offsetY = Mathf.Abs(j);
        //SetBounds(width + Mathf.Abs(i), height + Mathf.Abs(j), offsetX, offsetY);
    }

}
