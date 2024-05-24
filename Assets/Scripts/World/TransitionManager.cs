using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    GameObject plr;
    public static TransitionManager Instance;
    private static BuildGrid currentShip;
    private static Vector2 lastPlayerPos;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        plr = GameObject.FindGameObjectWithTag("Player");
        if (currentShip != null)
        {
            Ship ship = BuildingSystem.Instance.RespawnShipAtPlayer(currentShip, lastPlayerPos);
            ship.ChangeGravityScale(0f);
            currentShip = null;
        }
    }
    public void LoadSpace()
    {
        currentShip = BuildingSystem.Instance.GetShipAtPosition(plr.transform.position)?.ship;
        lastPlayerPos = plr.transform.position;
        SceneManager.LoadScene("SpaceScene");
    }
}
