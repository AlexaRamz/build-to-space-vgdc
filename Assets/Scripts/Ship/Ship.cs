using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    private GameObject player => PlayerMovement.Instance.gameObject;
    private Rigidbody2D rb;
    public BuildGrid ship;
    public int width => ship.Width;
    public int height => ship.Height;
    AudioManager audioManager;
    BuildingSystem buildSys;

    Build rocketBlock;

    public void SetUpShip(BuildGrid thisShip)
    {
        rb = GetComponent<Rigidbody2D>();
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

        //newShip.transform.position -= new Vector3(newShip.ship.Width / 2, 0);
        //newShip.transform.position -= (Vector3)offset.RotatedBy(gameObject.transform.eulerAngles.z * Mathf.Deg2Rad); //this is a system for readjusting the position of the ship when new blocks are added. Right now it is very finicky

        buildSys.SpawnObjects(ship, transform);
    }

    // Update the ship grid when blocks are added or deleted
    public void UpdateShip()
    {
        ship.bottomLeft = transform.position;
        ship.ClampBounds();
        ship.AddSizeAll();
        transform.position = ship.bottomLeft;
        buildSys.RepositionObjects(ship, transform);
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
    private void FixedUpdate()
    {
        rb.mass = ship.count;
        if (player != null && ship.PositionIsWithinGrid(player.transform.position)) //Player can hold right click to fly the ship while inside of it
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
                            ship.GridtoWorldPos(tile.Key), ForceMode2D.Impulse);
                    }
                }
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