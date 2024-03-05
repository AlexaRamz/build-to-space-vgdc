using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Bullet : MonoBehaviour
{
    public enum DamageOrigin { Player, Enemy };
    public DamageOrigin BulletOrigin = DamageOrigin.Player;
    // How fast projectile travels
    [Range (1,10)]
    [SerializeField] public float speed = 10f;

    // Lifetime of projectile (disappears after 3 seconds)
    [Range (1,10)]
    [SerializeField] public float lifeTime = 3f;


    [SerializeField]
    public int damage = 2;
    private Rigidbody2D rb;

    public bool ConstantVelocity = true;
    public bool DoesntDespawn;
    public bool OnlyDespawnWhenNotVisible;
    public bool ChasePlayers;


    GameObject player;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if(!DoesntDespawn)
            StartCoroutine(DelayedDeath());

        if(ChasePlayers)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
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
    bool isVisible = true;
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
        if (ConstantVelocity)
        {
            rb.velocity = transform.up * speed;
        }
        if(ChasePlayers)
        {
            transform.right = player.transform.position - transform.position;
            transform.eulerAngles -= new Vector3(0, 0, 90f);

        }
    }

        // Handle bullet collisions
        private void OnTriggerEnter2D(Collider2D other)
    {
        if (damage != 0)
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
        }
    }
}
