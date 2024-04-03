using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

public class Ship : MonoBehaviour
{
    private GameObject player => PlayerMovement.Instance.gameObject;
    public static List<Ship> LoadedShips = new List<Ship>();
    private Rigidbody2D rb;
    public BuildGrid ship;
    public int width => ship.width;
    public int height => ship.height;
    AudioManager audioManager;
    BuildingSystem buildSys;

    Build rocketBlock;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (ship == null)
            ship = new BuildGrid(0, 0, new Vector2Int(0, 0));
        // parent of ship is gameObject
        LoadedShips.Add(this);
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        buildSys = BuildingSystem.Instance;
        rocketBlock = (Build)Resources.Load("Builds/Data/Ship/Rocket");
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
        SetBounds(width + Mathf.Abs(i), height + Mathf.Abs(j), offsetX, offsetY);
    }
    public void SetBounds(int width, int height, int offsetX = 0, int offsetY = 0)
    {
        BuildGrid newTiles = new BuildGrid(width, height, new Vector2Int(0, 0));
        BuildGrid oldTiles = ship;

        gameObject.transform.position -= (Vector3)new Vector2(offsetX, offsetY).RotatedBy(gameObject.transform.eulerAngles.z * Mathf.Deg2Rad); //this is a system for readjusting the position of the ship when new blocks are added. Right now it is very finicky
        for(int i = 0; i < newTiles.width; i++)
        {
            for(int j = 0; j < newTiles.height; j++)
            {
                if (oldTiles.IsWithinGrid(new Vector2Int(i, j))) //Tiles within old boundaries
                {
                    BuildObject oldTile = oldTiles.GetValue(new Vector2Int(i - offsetX, j - offsetY));
                    if (oldTile != null)
                    {
                        newTiles.SetValue(new Vector2Int(i, j), oldTile);
                        oldTile.gridObject.transform.localPosition += new Vector3(offsetX, offsetY);
                    }
                }
                else if(!hasSetUp && (i == 0 || j == 0 || i == newTiles.width - 1 || j == newTiles.height - 1)) //New tiles
                {
                    ship.SetValue(new Vector2Int(i, j), new BuildObject(buildSys.buildCatalog.categories[0].builds[0], 0));
                }
            }
        }
        ship = newTiles;
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
                hasSetUp = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddSize(-1, 0);
        }
        if (transform.childCount <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void FixedUpdate()
    {
        rb.mass = transform.childCount;
        if (player != null)
        {
            Vector2Int playerInShip = ConvertPositionToShipCoordinates(player.transform.position, false);
            if (PositionInBounds(playerInShip)) //Player can hold right click to fly the ship while inside of it
            {
                if (Input.GetMouseButton(1))
                {
                    Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 toMouse = position - (Vector2)player.transform.position;
                    toMouse = toMouse.normalized;
                    rb.AddForce(toMouse * 0.1f * rb.mass, ForceMode2D.Impulse);

                    ///Searches for thrusters to apply force using
                    foreach (KeyValuePair<Vector2Int, BuildObject> tile in ship.gridObjects)
                    {
                        if (tile.Value != null && tile.Value.build == rocketBlock)
                        {
                            Debug.Log("Found a thruster: " + "(" + tile.Key.x + ", " + tile.Key.y + ")");
                            Rotation rotation = tile.Value.GetRotation();
                            Vector2 initialVector = Vector2.up * 3;
                            rb.AddForceAtPosition(initialVector.RotatedBy((transform.rotation.eulerAngles.z + rotation.DegRotation) * Mathf.Deg2Rad),
                                ConvertShipCoordinatesToPosition(tile.Key), ForceMode2D.Impulse);
                        }
                    }
                    audioManager.PlaySFX(audioManager.rocket);
                }
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            audioManager.StopSFX();
        }
    }
    private void OnDestroy()
    {
        LoadedShips.Remove(this);
    }
    public bool PositionInBounds(Vector2Int position)
    {
        return position.x >= 0 && position.y >= 0 && position.x < width && position.y < height;
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