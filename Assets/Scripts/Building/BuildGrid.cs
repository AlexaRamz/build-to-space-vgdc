using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildGrid
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int count { get; private set; }
    public Vector2 bottomLeft;
    public float rotation;

    public Dictionary<Vector2Int, BuildObject> gridObjects;

    public BuildGrid(Vector2 bottomLeft, int width = int.MaxValue, int height = int.MaxValue, float rotation = 0)
    {
        Width = width;
        Height = height;
        count = 0;
        this.bottomLeft = bottomLeft;
        this.rotation = rotation;

        gridObjects = new Dictionary<Vector2Int, BuildObject>();
    }
    public BuildGrid(BuildGrid toCopy, bool withGameObjects = true)
    {
        Width = toCopy.Width;
        Height = toCopy.Height;
        count = toCopy.count;
        bottomLeft = toCopy.bottomLeft;
        rotation = toCopy.rotation;

        gridObjects = new Dictionary<Vector2Int, BuildObject>();
        foreach (KeyValuePair<Vector2Int, BuildObject> o in toCopy.gridObjects)
        {
            gridObjects[o.Key] = o.Value.Clone();
            if (!withGameObjects)
            {
                gridObjects[o.Key].gridObject = null;
            }
        }
    }
    public BuildGrid Clone(bool withGameObjects = true)
    {
        BuildGrid newGrid = new BuildGrid(this, withGameObjects);
        return newGrid;
    }

    private Vector2Int WorldtoGridPos(Vector3 pos)
    {
        Vector2 newWorldPos = ((Vector2)pos).RotatedBy(-rotation * Mathf.Deg2Rad, bottomLeft) - bottomLeft;
        Vector2Int gridPos = new Vector2Int(Mathf.FloorToInt(newWorldPos.x), Mathf.FloorToInt(newWorldPos.y));
        return gridPos;
    }
    public Vector3 GridtoWorldPos(Vector2 pos)
    {
        Vector2 worldPos = bottomLeft + pos.RotatedBy(rotation * Mathf.Deg2Rad);
        return worldPos;
    }
    public Vector3 GridtoWorldAligned(Vector2Int gridPos)
    {
        Vector2 alignedGridPos = gridPos + new Vector2(0.5f, 0.5f);
        Vector2 alignedWorldPos = GridtoWorldPos(alignedGridPos);
        return alignedWorldPos;
    }
    public Vector3 WorldtoAligned(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldtoGridPos(worldPos);
        Vector3 alignedWorldPos = GridtoWorldAligned(gridPos);
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
        count++;
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
        count--;
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
    public bool PositionHasNullAdjacent(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldtoGridPos(worldPos);
        return HasNullAdjacent(gridPos);
    }
    private bool HasNullAdjacent(Vector2Int gridPos)
    {
        Vector2Int[] adjShifts = {
            new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1)
        };
        foreach (Vector2Int shift in adjShifts)
        {
            Vector2Int adjPos = gridPos + shift;
            if (GetValue(adjPos) == null)
                return true;
        }
        return false;
    }
    public bool PositionIsAtEdge(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldtoGridPos(worldPos);
        return gridPos.x == 0 || gridPos.x == Width - 1 || gridPos.y == 0 || gridPos.y == Height - 1;
    }

    private void ShiftKeys(Vector2Int offset)
    {
        if (offset == Vector2Int.zero) return;

        Dictionary<Vector2Int, BuildObject> newGridObjects = new Dictionary<Vector2Int, BuildObject>();
        foreach (KeyValuePair<Vector2Int, BuildObject> p in gridObjects)
        {
            newGridObjects[p.Key + offset] = p.Value;
        }
        gridObjects = newGridObjects;
    }
    private void ShiftPosition(Vector2Int offset)
    {
        bottomLeft = GridtoWorldPos(offset);
    }
    public void ClampBounds()
    {
        if (gridObjects.Count == 0)
        {
            Width = Height = 0;
            return;
        }

        Vector2Int minGridPos = new Vector2Int(int.MaxValue, int.MaxValue);
        Vector2Int maxGridPos = Vector2Int.zero;
        foreach (KeyValuePair<Vector2Int, BuildObject> p in gridObjects)
        {
            if (p.Key.x < minGridPos.x)
            {
                minGridPos.x = p.Key.x;
            }
            if (p.Key.y < minGridPos.y)
            {
                minGridPos.y = p.Key.y;
            }
            if (p.Key.x > maxGridPos.x)
            {
                maxGridPos.x = p.Key.x;
            }
            if (p.Key.y > maxGridPos.y)
            {
                maxGridPos.y = p.Key.y;
            }
        }
        if (minGridPos == Vector2Int.zero) return;

        Width = maxGridPos.x - minGridPos.x + 1;
        Height = maxGridPos.y - minGridPos.y + 1;

        ShiftKeys(-minGridPos);
        ShiftPosition(minGridPos);
    }
    public void AddSizeAll()
    {
        Width += 2;
        Height += 2;
        Vector2Int offset = new Vector2Int(1, 1);
        ShiftKeys(offset);
        ShiftPosition(-offset);
    }
    List<Vector2Int> GetNeighborPositions(Vector2Int gridPos)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        Vector2Int[] neighborOffsets = { new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1) };
        foreach (Vector2Int offset in neighborOffsets)
        {
            if (GetValue(gridPos + offset) != null)
            {
                positions.Add(gridPos + offset);
            }
        }
        return positions;
    }
    bool HasPathToGround(Vector2Int gridPos)
    {
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        bool DFS(Vector2Int pos)
        {
            visited.Add(pos);
            BuildObject value = GetValue(pos);
            if (value != null && value.build == null)
            {
                Debug.Log("true");
                return true;
            }
            List<Vector2Int> neighborPositions = GetNeighborPositions(pos);
            foreach (Vector2Int neighbor in neighborPositions)
            {
                Debug.Log(neighbor);
                if (!visited.Contains(neighbor))
                {
                    if (DFS(neighbor))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        return DFS(gridPos);
    }
    public bool CheckCollapseOnDelete(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldtoGridPos(worldPos);
        BuildObject removed = GetValue(gridPos);
        RemoveValue(gridPos);

        List<Vector2Int> neighborPositions = GetNeighborPositions(gridPos);
        foreach (Vector2Int neighbor in neighborPositions)
        {
            if (!HasPathToGround(neighbor))
            {
                SetValue(gridPos, removed);
                return true;
            }
        }
        SetValue(gridPos, removed);
        return false;
    }
}
