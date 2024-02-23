using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Ship : MonoBehaviour
{
    public static List<Ship> LoadedShips = new List<Ship>();
    public Rigidbody2D RB;
    public BuildArray ship;
    public int Width => ship.Width;
    public int Height => ship.Height;
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
        if (i < 0)
            offsetX = Mathf.Abs(i);
        if (j < 0)
            offsetY = Mathf.Abs(j);
        SetBounds(Width + Mathf.Abs(i), Height + Mathf.Abs(j), offsetX, offsetY);
    }
    public void SetBounds(int width, int height, int offsetX = 0, int offsetY = 0)
    {
        BuildingSystem bs = BuildingSystem.Instance;
        Rotation rot = bs.categories[0].builds[0].rotations[0];
        Tile[,] newTiles = new Tile[width, height];
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                newTiles[i, j] = new Tile();
            }
        }
        Tile[,] oldTiles = ship.tile;
        gameObject.transform.position -= (Vector3)new Vector2(offsetX, offsetY).RotatedBy(gameObject.transform.eulerAngles.z * Mathf.Deg2Rad); //this is a system for readjusting the position of the ship when new blocks are added. Right now it is very finicky
        for(int i = 0; i < newTiles.GetLength(0); i++)
        {
            for(int j = 0; j < newTiles.GetLength(1); j++)
            {
                if (i < oldTiles.GetLength(0) + offsetX && i >= offsetX && j < oldTiles.GetLength(1) + offsetY && j >= offsetY) //Tiles within old boundaries
                {
                    newTiles[i, j] = oldTiles[i - offsetX, j - offsetY];
                    if(newTiles[i, j].HasTile)
                        newTiles[i, j].transform.localPosition += new Vector3(offsetX, offsetY);
                }
                else if(!hasSetUp && (i == 0 || j == 0 || i == newTiles.GetLength(0) - 1 || j == newTiles.GetLength(1) - 1)) //New tiles
                {
                    ship.PlaceBlock(ref newTiles, i, j, bs.categories[0].builds[0], rot);
                }
            }
        }
        ship.tile = newTiles;
    }
    private void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        if(ship == null)
            ship = new BuildArray(gameObject, 0, 0, 0, 0);
        LoadedShips.Add(this);
    }
    private static bool hasSetUp = false;
    void Update()
    {
        if(!hasSetUp)
        {
            if(BuildingSystem.Instance != null)
            {
                SetBounds(5, 5);
                AddSize(-2, -2);
                AddSize(2, 2);
                RB.mass = transform.childCount;
                hasSetUp = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddSize(-1, 0);
        }
        /*if(Input.GetMouseButtonDown(1))
        { 
            Vector2Int pos = ConvertPositionToShipCoordinates(Input.mousePosition, true);
            Debug.Log(pos);
            Debug.Log(Width);
            Debug.Log(Height);
            if (PositionInBounds(pos))
            {
                Debug.Log(ship.PlaceBlock(pos.x, pos.y, BuildingSystem.Instance.GetBuild(), BuildingSystem.Instance.currentInfo.GetRotation()));

            }
        }*/
        if(transform.childCount <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        LoadedShips.Remove(this);
    }
    public bool PositionInBounds(Vector2Int position)
    {
        return position.x >= 0 && position.y >= 0 && position.x < Width && position.y < Height;
    }
    public Vector2Int ConvertPositionToShipCoordinates(Vector2 position, bool fromScreenPosition = false)
    {
        if(fromScreenPosition)
        {
            position = Camera.main.ScreenToWorldPoint(position);
        } 
        //debugNum++;
        Vector2 shipPos = (Vector2)gameObject.transform.position - new Vector2(0.5f, 0.5f).RotatedBy(gameObject.transform.eulerAngles.z * Mathf.Deg2Rad);
        Vector2 mousePos = ((Vector2)position).RotatedBy(-gameObject.transform.eulerAngles.z * Mathf.Deg2Rad, shipPos) - shipPos;
        Vector2Int worldPos = new Vector2Int(Mathf.FloorToInt(mousePos.x), Mathf.FloorToInt(mousePos.y));
        //Debug.Log(debugNum + ": " + worldPos + "---" + new Vector2(0.5f, 0.5f).RotatedBy(gameObject.transform.eulerAngles.z * Mathf.Deg2Rad));
        return worldPos;
    }
    public Vector2 ConvertShipCoordinatesToPosition(Vector2Int gridPosition)
    {
        return (Vector2)transform.position + new Vector2(gridPosition.x, gridPosition.y).RotatedBy(gameObject.transform.eulerAngles.z * Mathf.Deg2Rad);
    }
}
