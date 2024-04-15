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

        //Vector3 offset = ship.ClampBounds();
        //transform.position += offset;
        ship.bottomLeft = transform.position;

        //newShip.transform.position -= new Vector3(newShip.ship.Width / 2, 0);
        //newShip.transform.position -= (Vector3)offset.RotatedBy(gameObject.transform.eulerAngles.z * Mathf.Deg2Rad); //this is a system for readjusting the position of the ship when new blocks are added. Right now it is very finicky
        //ship.ship.ShiftObjects(offset);

        //ship.ship.AddSize(1, 1); //Expand the ship size by 1 in each direction to allow placing around the ship
        //ship.ship.AddSize(-1, -1);

        buildSys.SpawnObjects(ship, transform);
    }

    public void UpdateShip()
    {
        //Vector3 offset = ship.ClampBounds();
        //transform.position += offset;
        //buildSys.ShiftObjects(ship, offset);
        ship.bottomLeft = transform.position;
        ship.rotation = transform.eulerAngles.z;
    }

    void Update()
    {
        ship.bottomLeft = transform.position;
        ship.rotation = transform.eulerAngles.z;

        if (buildSys != null)
        {
            UpdateShip();
            //AddSize(-2, -2);
            //AddSize(2, 2);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            //AddSize(-1, 0);
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
            if (ship.PositionIsWithinGrid(player.transform.position)) //Player can hold right click to fly the ship while inside of it
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
                                ship.GridtoWorldAligned(tile.Key), ForceMode2D.Impulse);
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
        ShipBuilding.loadedShips.Remove(this);
    }
}