using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public float scrollSpeedX = 0.5f;
    public float scrollSpeedY = 0.5f;

    MeshRenderer mr;
    Material mat;

    private void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mat = mr.material;
    }
    void LateUpdate()
    {
        float offsetX = (transform.position.x / transform.localScale.x) / scrollSpeedX;
        float offsetY = (transform.position.y / transform.localScale.y) / scrollSpeedY;
        if (scrollSpeedX <= 0f)
        {
            offsetX = 0f;
        }
        if (scrollSpeedY <= 0f)
        {
            offsetY = 0f;
        }
        mat.mainTextureOffset = new Vector2(offsetX, offsetY);
    }

}

