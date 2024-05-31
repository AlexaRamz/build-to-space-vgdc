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
    [SerializeField] SpriteRenderer overlay;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        plr = GameObject.FindGameObjectWithTag("Player");
        if (SceneManager.GetActiveScene().name == "SpaceScene")
        {
            plr.GetComponent<PlayerMovement>().worldGravityScale = 0f;
        }
        if (currentShip != null)
        {
            Ship ship = BuildingSystem.Instance.RespawnShipAtPlayer(currentShip, lastPlayerPos);
            currentShip = null;
        }
    }
    public void ResetOverlay()
    {
        overlay.color = new Color(0f, 0f, 0f, 0f);
    }
    public void FadeOverlay(float value, Color color)
    {
        float transparency = Mathf.Clamp(value, 0.0f, 1.0f);
        overlay.color = new Color(color.r, color.g, color.b, transparency);
    }
    public void SaveCurrentShip()
    {
        currentShip = BuildingSystem.Instance.GetShipAtPosition(plr.transform.position)?.ship;
        lastPlayerPos = plr.transform.position;
    }
}
