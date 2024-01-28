using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    GameObject plr;
    public float minValueY;
    private void Start()
    {
        plr = GameObject.Find("Player");
    }
    private void Update()
    {
        transform.position = new Vector3(plr.transform.position.x, Mathf.Clamp(plr.transform.position.y, minValueY, Mathf.Infinity), transform.position.z);
        

    }
}
