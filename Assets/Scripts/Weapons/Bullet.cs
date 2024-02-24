using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // How fast projectile travels
    [Range(1, 10)]
    public float speed = 10f;

    // Lifetime of projectile (disappears after 3 seconds)
    [Range(1, 10)]
    public float lifeTime = 3f;

    // Damage of projectile
    public int damage = 1;

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
    
    // Handle bullet collisions
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            if (other.gameObject.tag == "Enemy")
            {
                other.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
