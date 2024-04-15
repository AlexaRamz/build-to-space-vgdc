using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildGrid
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Vector2 bottomLeft;
    public float rotation;

    public Dictionary<Vector2Int, BuildObject> gridObjects;

    public BuildGrid(Vector2 bottomLeft, int width = int.MaxValue, int height = int.MaxValue, float rotation = 0)
    {
        Width = width;
        Height = height;
        this.bottomLeft = bottomLeft;
        this.rotation = rotation;

        gridObjects = new Dictionary<Vector2Int, BuildObject>();
    }
    public BuildGrid Clone(bool withGameObjects)
    {
        BuildGrid newGrid = new BuildGrid(bottomLeft, Width, Height);
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
    private Vector2Int WorldtoGridPos(Vector3 pos)
    {
        Vector2 newWorldPos = ((Vector2)pos).RotatedBy(-rotation * Mathf.Deg2Rad, bottomLeft) - bottomLeft;
        Vector2Int gridPos = new Vector2Int(Mathf.FloorToInt(newWorldPos.x), Mathf.FloorToInt(newWorldPos.y));
        return gridPos;
    }
    private Vector3 GridtoWorldPos(Vector2 pos)
    {
        Vector2 worldPos = bottomLeft + new Vector2(pos.x, pos.y).RotatedBy(rotation * Mathf.Deg2Rad);
        return worldPos;
    }
    public Vector3 GridtoWorldAligned(Vector2Int pos)
    {
        Vector2 worldPos = GridtoWorldPos(pos);
        return worldPos + new Vector2(0.5f, 0.5f);
    }
    public Vector3 WorldtoAligned(Vector3 pos)
    {
        Vector2 gridPos = WorldtoGridPos(pos);
        Vector2 alignedGridPos = gridPos + new Vector2(0.5f, 0.5f);
        Vector2 alignedWorldPos = GridtoWorldPos(alignedGridPos);
        return alignedWorldPos;
    }

    public bool PositionIsWithinGrid(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldtoGridPos(worldPos);
        return IsWithinGrid(gridPos);
    }
    private bool IsWithinGrid(Vector2Int gridPos)
    {
        return gridPos.x >= 0 && gridPos.x < Width && gridPos.y >= 0 && gridPos.y < Height;
    }

    public BuildObject GetValueAtPosition(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldtoGridPos(worldPos);
        return GetValue(gridPos);
    }
    private BuildObject GetValue(Vector2Int gridPos)
    {
        if (!IsWithinGrid(gridPos) || !gridObjects.ContainsKey(gridPos)) return null;
        return gridObjects[gridPos];
    }

    public bool SetValueAtPosition(Vector3 worldPos, BuildObject value)
    {
        Vector2Int gridPos = WorldtoGridPos(worldPos);
        return SetValue(gridPos, value);
    }
    private bool SetValue(Vector2Int gridPos, BuildObject value)
    {
        if (!IsWithinGrid(gridPos)) return false;
        gridObjects[gridPos] = value;
        return true;
    }

    public bool RemoveValueAtPosition(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldtoGridPos(worldPos);
        return RemoveValue(gridPos);
    }
    private bool RemoveValue(Vector2Int gridPos)
    {
        if (!IsWithinGrid(gridPos) || !gridObjects.ContainsKey(gridPos)) return false;
        gridObjects.Remove(gridPos);
        return true;
    }

    public bool PositionHasAdjacent(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldtoGridPos(worldPos);
        return HasAdjacent(gridPos);
    }
    private bool HasAdjacent(Vector2Int gridPos)
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
    public bool PositionIsAtEdge(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldtoGridPos(worldPos);
        return gridPos.x == 0 || gridPos.x == Width - 1 || gridPos.y == 0 || gridPos.y == Height - 1;
    }

    public Vector3 ClampBounds()
    {
        if (gridObjects.Count == 0)
        {
            Width = Height = 0;
            return Vector3.zero;
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
        Width = newWidth;
        Height = newHeight;

        if (offset == Vector2Int.zero) return Vector3.zero;

        Dictionary<Vector2Int, BuildObject> newGridObjects = new Dictionary<Vector2Int, BuildObject>();
        foreach (KeyValuePair<Vector2Int, BuildObject> p in gridObjects)
        {
            newGridObjects[p.Key - offset] = p.Value;
        }
        gridObjects = newGridObjects;

        Vector2 oldBottomLeft = bottomLeft;
        bottomLeft = GridtoWorldPos(offset);
        return bottomLeft - oldBottomLeft;
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
        //SetBounds(Width + Mathf.Abs(i), Height + Mathf.Abs(j), offsetX, offsetY);
    }

}
