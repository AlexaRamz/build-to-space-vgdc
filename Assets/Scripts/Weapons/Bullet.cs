using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum DamageOrigin { Player, Enemy };
    public DamageOrigin BulletOrigin = DamageOrigin.Player;
    // How fast projectile travels
    [Range (1,10)]
    [SerializeField] private float speed = 10f;

    // Lifetime of projectile (disappears after 3 seconds)
    [Range (1,10)]
    [SerializeField] private float lifeTime = 3f;

    private Rigidbody2D rb;

<<<<<<< Updated upstream
    private void Start() 
=======
    public bool ConstantVelocity = true;
    public bool OnlyDespawnWhenNotVisible;


    private void Start()
>>>>>>> Stashed changes
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(DelayedDeath());


        rb.velocity = transform.up * speed;
    }
    IEnumerator DelayedDeath()
    {
        yield return new WaitForSeconds(lifeTime);
        while(OnlyDespawnWhenNotVisible&& isVisible)
        {
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
    bool isVisible = false;
    private void OnBecameVisible()
    {
        isVisible = true;
    }
    private void OnBecameInvisible()
    {
        isVisible = false;
    }

    private void FixedUpdate()
    {
<<<<<<< Updated upstream
        rb.velocity = transform.up * speed;
=======
        if (ConstantVelocity)
        {
            rb.velocity = transform.up * speed;
        }
    }

        // Handle bullet collisions
        private void OnTriggerEnter2D(Collider2D other)
    {
        if (BulletOrigin == DamageOrigin.Player)
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
        else
        {//Enemy shot this bullet 
            if (other.gameObject.layer != LayerMask.NameToLayer("NPC"))
            {
                if (other.gameObject.tag == "Player")
                {
                    other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
                }
                Destroy(gameObject);
            }

        }
>>>>>>> Stashed changes
    }
}
