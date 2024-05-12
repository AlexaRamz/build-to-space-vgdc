// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class ScrollingBackground : MonoBehaviour
// {
//     public float scrollSpeed = 0.5f;
//     void Start()
//     {
//     }
//     // Update is called once per frame
//     void Update()
//     {
//         MeshRenderer mr = GetComponent<MeshRenderer>();

//         Material mat = mr.material;
//         mat.mainTexture.wrapMode = TextureWrapMode.Repeat;

//         float offset = Time.time * scrollSpeed;
//         mat.mainTextureOffset = new Vector2(offset/2, 0);

//         // Vector2 offset = mat.mainTextureOffset;

//         // offset.x = Time.deltaTime;

//         // mat.mainTextureOffset = offset * 100;
//     }

// }