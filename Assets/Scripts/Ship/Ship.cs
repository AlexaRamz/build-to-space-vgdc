using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Ship : MonoBehaviour
{
    private Rigidbody2D RB;
    private Tile[,] tiles = new Tile[0, 0];
    private List<GameObject> myObj = new List<GameObject>();
    private int Width => tiles.GetLength(0);
    private int Height => tiles.GetLength(1);
    /// <summary>
    /// Increases the size of the ship. Negative numbers grow the ship to the left/bottom. Positive numbers grow the ship to the right/top
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    private void AddSize(int i = 0, int j = 0)
    {
        int offsetX = 0;
        int offsetY = 0;
        if (i < 0)
            offsetX = Mathf.Abs(i);
        if (j < 0)
            offsetY = Mathf.Abs(j);
        SetBounds(Width + Mathf.Abs(i), Height + Mathf.Abs(j), offsetX, offsetY);
    }
    private void SetBounds(int width, int height, int offsetX = 0, int offsetY = 0)
    {
        BuildingSystem bs = BuildingSystem.Instance;
        Rotation rot = bs.categories[0].builds[0].rotations[0];
        Tile[,] newTiles = new Tile[width, height];
        foreach(GameObject obj in myObj)
        {
            obj.transform.localPosition += new Vector3(offsetX, offsetY);
        }
        gameObject.transform.position -= (Vector3)new Vector2(offsetX, offsetY).RotatedBy(gameObject.transform.eulerAngles.z * Mathf.Deg2Rad); //this is a system for readjusting the position of the ship when new blocks are added. Right now it is very finicky
        for(int i = 0; i < newTiles.GetLength(0); i++)
        {
            for(int j = 0; j < newTiles.GetLength(1); j++)
            {
                if (i < tiles.GetLength(0) + offsetX && i >= offsetX && j < tiles.GetLength(1) + offsetY && j >= offsetY)
                {
                    newTiles[i, j] = tiles[i - offsetX, j - offsetY];
                }
                else if(i == 0 || j == 0 || i == newTiles.GetLength(0) - 1 || j == newTiles.GetLength(1) - 1)
                {
                    Vector3 pos = new Vector3(i, j); //This block placing system needs to be unified with the placement system in BuildingSystem. But to do that would require a TON of refactoring...
                    if (!newTiles[i, j].HasTile)
                    {
                        GameObject clone = rot.Object;
                        GameObject obj;
                        if (clone == null)
                        {
                            if (bs.categories[0].builds[0].depth == Build.DepthLevel.MidGround)
                                clone = bs.buildTemplate;
                            else
                                clone = bs.backBuildTemplate;
                        }
                        obj = Instantiate(clone, Vector3.zero, gameObject.transform.rotation, gameObject.transform);
                        obj.transform.localPosition = pos;
                        if (rot.sprite != null)
                        {
                            SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
                            renderer.sprite = rot.sprite;
                        }
                        newTiles[i, j].obj = obj;
                        newTiles[i, j].info = new BuildInfo();

                        if (bs.categories[0].builds[0].depth == Build.DepthLevel.MidGround)
                        {
                            PolygonCollider2D collider = obj.AddComponent<PolygonCollider2D>();
                            collider.usedByComposite = true;
                        }
                        myObj.Add(obj);
                    }
                }
            }
        }
        tiles = newTiles;
    }
    private void PlaceBlock(int i, int j, Rotation rot)
    {
        BuildingSystem bs = BuildingSystem.Instance;
        Vector3 pos = new Vector3(i, j); //This block placing system needs to be unified with the placement system in BuildingSystem. But to do that would require a TON of refactoring...
        if (!tiles[i, j].HasTile)
        {
            GameObject clone = rot.Object;
            GameObject obj;
            if (clone == null)
            {
                if (bs.categories[0].builds[0].depth == Build.DepthLevel.MidGround)
                    clone = bs.buildTemplate;
                else
                    clone = bs.backBuildTemplate;
            }
            obj = Instantiate(clone, Vector3.zero, gameObject.transform.rotation, gameObject.transform);
            obj.transform.localPosition = pos;
            if (rot.sprite != null)
            {
                SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
                renderer.sprite = rot.sprite;
            }
            tiles[i, j].obj = obj;
            tiles[i, j].info = new BuildInfo();

            if (bs.categories[0].builds[0].depth == Build.DepthLevel.MidGround)
            {
                PolygonCollider2D collider = obj.AddComponent<PolygonCollider2D>();
                collider.usedByComposite = true;
            }
            myObj.Add(obj);
        }
    }
    private void Start()
    {
        RB = GetComponent<Rigidbody2D>();
    }
    bool hasSetUp = false;
    void Update()
    {
        if(!hasSetUp)
        {
            if(BuildingSystem.Instance != null)
            {
                SetBounds(5, 5);
                AddSize(-2, -2);
                AddSize(2, 2);
                RB.mass = Width * Height;
                hasSetUp = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddSize(-1, 0);
        }
        if(Input.GetMouseButtonDown(1))
        {
            ConvertPositionToShipCoordinates();
        }
    }
    private int debugNum = 0;
    private void ConvertPositionToShipCoordinates()
    {
        debugNum++;
        Vector2 shipPos = (Vector2)gameObject.transform.position - new Vector2(0.5f, 0.5f).RotatedBy(gameObject.transform.eulerAngles.z * Mathf.Deg2Rad);
        Vector2 mousePos = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition)).RotatedBy(-gameObject.transform.eulerAngles.z * Mathf.Deg2Rad, shipPos) - shipPos;
        Vector2 worldPos = new Vector2(Mathf.FloorToInt(mousePos.x), Mathf.FloorToInt(mousePos.y));
        Debug.Log(debugNum + ": " + worldPos + "---" + new Vector2(0.5f, 0.5f).RotatedBy(gameObject.transform.eulerAngles.z * Mathf.Deg2Rad));
    }
}
