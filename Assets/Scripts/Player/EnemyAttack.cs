using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
using static EnemyMove;

public class EnemyAttack : MonoBehaviour
{

    //All null attributes will be ignored.

    [Header("Death")]

    [Tooltip("The object to spawn when the enemy dies.")]
    public GameObject createOnDeath;
    [Tooltip("Requires the death object to only spawn on the ground.")]
    public bool snapOnDeathCreationToGround;


    [Header("Ranged")]

    [Tooltip("The object the enemy will attempt to shoot at the player.")]
    public GameObject bulletPrefab;

    [Tooltip("The detection range for when the enemy will shoot at the player.")]
    public float shootRange;


    [Tooltip("How large bursts are when firing.")]
    public float burstSize = 1f;
    [Tooltip("How often bullets can be fired at the player.")]
    public float shootDelay = 1f;


    [Tooltip("Where the bullets emerge from when fired.")]
    public Transform firingPoint;

    [Header("Collision")]


    [Tooltip("The damage dealt to the player on collision.")]
    public int damage = 2;

    [Tooltip("How often collision damage can be dealt to the player.")]
    public float attackDelay = 1f;


    GameObject player;

    private bool canDamage = true;
    private bool canShoot = true;



    int shotsFired;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if(firingPoint==null)
        {
            firingPoint = transform;
        }
    }

    public bool HasClearShot()
    {
        Vector2 pos = player.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (pos - new Vector2(transform.position.x, transform.position.y)).normalized, Vector2.Distance(transform.position, pos), ~LayerMask.GetMask("NPC", "Player"));

        if (hit.collider != null)
        {
            //Something is in the way!
            return false;
        }
        return true;
    }

    private void FixedUpdate()
    {
        if (bulletPrefab != null)
        {
            if (canShoot && Vector2.Distance(transform.position, player.transform.position) <= shootRange && HasClearShot())
            {


                var tempRot = transform.localRotation;

                transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.x - transform.position.x) * Mathf.Rad2Deg);

                Instantiate(bulletPrefab, firingPoint.position, firingPoint.rotation);


                canShoot = false;
                shotsFired++;

                if (shotsFired >= burstSize)
                {
                    shotsFired = 0;
                    StartCoroutine(ShootDelay(shootDelay));
                }
                else
                {
                    StartCoroutine(ShootDelay(0.1f));
                }
                

                transform.localRotation = tempRot;
            }
        }
    }

    IEnumerator ShootDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canShoot = true;
    }
    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(attackDelay);
        canDamage = true;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (canDamage && collision.gameObject.tag == "Player")
        {
            canDamage = false;
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
            StartCoroutine(AttackDelay());
        }
    }
}
