using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    private PlayerMovement plr => PlayerMovement.Instance;
    private Rigidbody2D rb;
    public BuildGrid ship;
    public int width => ship.Width;
    public int height => ship.Height;
    AudioManager audioManager;
    BuildingSystem buildSys;

    Build rocketBlock;
    List<KeyValuePair<Vector2Int, BuildObject>> thrusters = new List<KeyValuePair<Vector2Int, BuildObject>>();

    public void SetUpShip(BuildGrid thisShip)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = plr.worldGravityScale;
        ship = thisShip;
        ShipBuilding.loadedShips.Add(this);
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        rocketBlock = (Build)Resources.Load("Builds/Data/Ship/Rocket");
        buildSys = BuildingSystem.Instance;

        ship.bottomLeft = transform.position;
        ship.ClampBounds();
        //Expand the ship size by 1 in each direction to allow placing around the ship
        ship.AddSizeAll();
        Debug.Log(ship.Width + "and" + ship.Height);

        transform.position = ship.bottomLeft;
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, ship.rotation);

        FindThrusters();

        buildSys.SpawnObjects(ship, transform);
    }

    // Update the ship grid when blocks are added or deleted
    public void UpdateShip(Vector2 changedPos)
    {
        if (ship.PositionIsAtEdge(changedPos))
        {
            ship.bottomLeft = transform.position;
            ship.ClampBounds();
            ship.AddSizeAll();
            transform.position = ship.bottomLeft;
            buildSys.RepositionObjects(ship, transform);
        }

        FindThrusters();
    }


    public void FindThrusters()
    {
        thrusters.Clear();
        foreach (KeyValuePair<Vector2Int, BuildObject> tile in ship.gridObjects)
        {
            if (tile.Value != null && tile.Value.build == rocketBlock)
            {
                Debug.Log("Found a thruster: " + "(" + tile.Key.x + ", " + tile.Key.y + ")");
                thrusters.Add(tile);
            }
        }
    }

    public void ChangeGravityScale(float scale)
    {
        rb.gravityScale = scale;
    }

    void Update()
    {
        ship.bottomLeft = transform.position;
        ship.rotation = transform.eulerAngles.z;

        if (transform.childCount <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void ApplyThrust(int thrusterNum)
    {
        if (thrusterNum >= 0 && thrusterNum < thrusters.Count)
        {
            KeyValuePair<Vector2Int, BuildObject> part = thrusters[thrusterNum];
            float angle = part.Value.GetRotation().DegRotation;
            Vector2 initialVector = Vector2.up * 3;
            Vector2 tilePos = ship.GridtoWorldPos(part.Key);
            Vector2 forcePoint = new Vector2(tilePos.x + 0.5f * Mathf.Cos((angle) * Mathf.Deg2Rad), tilePos.y + 0.5f * Mathf.Sin((angle) * Mathf.Deg2Rad));
            rb.AddForceAtPosition(initialVector.RotatedBy((transform.rotation.eulerAngles.z + angle) * Mathf.Deg2Rad),
                forcePoint, ForceMode2D.Impulse);
        }
    }
    public void ApplyThrustTowards(float degDirection)
    {
        foreach (KeyValuePair<Vector2Int, BuildObject> part in thrusters)
        {
            float angle = part.Value.GetRotation().DegRotation;
            if (angle == degDirection)
            {
                Vector2 initialVector = Vector2.up * 3;
                Vector2 tilePos = ship.GridtoWorldPos(part.Key);
                Vector2 forcePoint = new Vector2(tilePos.x + 0.5f * Mathf.Cos((angle) * Mathf.Deg2Rad), tilePos.y + 0.5f * Mathf.Sin((angle) * Mathf.Deg2Rad));
                rb.AddForceAtPosition(initialVector.RotatedBy((transform.rotation.eulerAngles.z + angle) * Mathf.Deg2Rad),
                    forcePoint, ForceMode2D.Impulse);
            }
        }
    }

    private void FixedUpdate()
    {
        rb.mass = ship.count;
        if (plr != null && ship.PositionIsWithinGrid(plr.transform.position)) //Player can hold right click to fly the ship while inside of it
        {
            if (Input.GetMouseButton(1))
            {
                Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 toMouse = position - (Vector2)plr.transform.position;
                toMouse = toMouse.normalized;
                rb.AddForce(toMouse * 0.1f * rb.mass, ForceMode2D.Impulse);

                audioManager.PlaySFX(audioManager.rocket);
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            audioManager.StopSFX();
        }
    }
    private void OnDestroy()
    {
        ShipBuilding.loadedShips.Remove(this);
    }
}