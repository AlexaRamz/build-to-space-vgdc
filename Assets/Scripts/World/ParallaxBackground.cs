using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ParallaxBackground : MonoBehaviour
{

    private float lenght;
    Vector2 startpos;
    public GameObject cam;
    public float parallaxEffect;
    float startCamPosY;

    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position;
        lenght = GetComponent<SpriteRenderer>().bounds.size.x;
        cam = Camera.main.gameObject;
        startCamPosY = cam.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float distX = cam.transform.position.x * parallaxEffect;
        float distY = (cam.transform.position.y - startCamPosY) * parallaxEffect;

        transform.position = new Vector3(startpos.x + distX, startpos.y - distY, transform.position.z);
        if (cam.transform.position.x - transform.position.x > 1.5 * lenght) startpos.x += 3 * lenght;
        else if (transform.position.x - cam.transform.position.x > 1.5 * lenght) startpos.x -= 3 * lenght;


    }
}
