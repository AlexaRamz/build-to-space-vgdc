using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainManager : MonoBehaviour
{
    public Tilemap ground;

    public static TerrainManager Instance;
    BuildingSystem buildSys;
    [SerializeField] private InventoryManager plrInv;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        buildSys = BuildingSystem.Instance;
    }
    public void AddGroundTiles()
    {
        if (ground == null) return;

        buildSys = BuildingSystem.Instance;
        BoundsInt tilemapBounds = ground.cellBounds;
        for (int x = tilemapBounds.min.x; x < tilemapBounds.max.x; x++)
        {
            for (int y = tilemapBounds.min.y; y < tilemapBounds.max.y; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);

                TileBase tile = ground.GetTile(cellPos);

                if (tile != null)
                {
                    buildSys.worldGrid.SetValueAtPosition(ground.CellToWorld(cellPos), new BuildObject(null));
                }
            }
        }
    }
    public static bool HasAdjacentTile(Vector3Int cellPosition, Tilemap tilemap)
    {
        Vector3Int[] neighborOffsets = { Vector3Int.up, Vector3Int.right, Vector3Int.down, Vector3Int.left };

        foreach (Vector3Int offset in neighborOffsets)
        {
            Vector3Int neighborPosition = cellPosition + offset;

            if (tilemap.HasTile(neighborPosition) && tilemap.GetTile(neighborPosition) != null)
            {
                return true;
            }
        }
        return false;
    }

    public void DeleteAtMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int clickedCell = ground.WorldToCell(mousePos);

        TileBase tile = ground.GetTile(clickedCell);
        Item resource = plrInv.GetResourceFromTile(tile);
        if (resource != null)
        {
            plrInv.AddItem(resource, 1);
        }

        if (ground.HasTile(clickedCell))
        {
            if (buildSys.DeleteObject(mousePos, buildSys.worldGrid, true))
            {
                ground.SetTile(clickedCell, null);
            }
        }
    }
}
