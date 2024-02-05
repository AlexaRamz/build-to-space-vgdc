using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // How fast projectile travels
    [Range (1,10)]
    [SerializeField] private float speed = 10f;

    // Lifetime of projectile (disappears after 3 seconds)
    [Range (1,10)]
    [SerializeField] private float lifeTime = 3f;

    private Rigidbody2D rb;

    private void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.up * speed;
    }
}
