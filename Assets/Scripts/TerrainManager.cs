using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainManager : MonoBehaviour
{
    public Tilemap ground;

    public static TerrainManager Instance;
    BuildingSystem buildSys;
    Inventory plrInv;
    Dictionary<string, ResourceType> tileNameToOre;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        buildSys = BuildingSystem.Instance;
        plrInv = Inventory.Instance;
        tileNameToOre = new Dictionary<string, ResourceType> { { "Ground", ResourceType.Copper }, { "Ground_1", ResourceType.Aluminum } };
    }

    public void AddGroundTiles()
    {
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
                    Debug.Log("tile");
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

        string tileName = ground.GetTile(clickedCell)?.name;
        if (tileName != null && tileNameToOre.ContainsKey(tileName))
        {
            plrInv.Collect(tileNameToOre[tileName]);
        }

        if (buildSys.DeleteObject(mousePos, buildSys.worldGrid))
        {
            ground.SetTile(clickedCell, null);
        }
    }
}
