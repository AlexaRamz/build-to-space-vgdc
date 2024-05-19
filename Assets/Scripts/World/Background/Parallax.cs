using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    float length;
    Vector2 startPos;
    public float magnitudeX = 0.8f;
    public float magnitudeY = 0.2f;
    GameObject cam;
    float startCamPosY;
    void Start()
    {
        startPos = transform.position;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        cam = Camera.main.gameObject;
        startCamPosY = cam.transform.position.y;
    }

    
    void LateUpdate()
    {
        float distX = cam.transform.position.x * magnitudeX;
        float distY = (cam.transform.position.y - startCamPosY) * magnitudeY;

        transform.position = new Vector3(startPos.x + distX, startPos.y - distY, transform.position.z);
        if (cam.transform.position.x - transform.position.x > 1.5 * length) startPos.x += 3 * length;
        else if (transform.position.x - cam.transform.position.x > 1.5 * length) startPos.x -= 3 * length;
    }
}
