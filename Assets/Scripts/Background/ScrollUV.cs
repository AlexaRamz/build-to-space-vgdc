using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowUV : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();

        Material mat = mr.material;
        mat.mainTexture.wrapMode = TextureWrapMode.Repeat;

        float offsetX = (transform.position.x/transform.localScale.x) / scrollSpeed;
        float offsetY = (transform.position.y/transform.localScale.y) / scrollSpeed;
        mat.mainTextureOffset = new Vector2(offsetX, offsetY);

        // Vector2 offset = mat.mainTextureOffset;

        // offset.x = Time.deltaTime;

        // mat.mainTextureOffset = offset * 100;

        
        
    }

}

