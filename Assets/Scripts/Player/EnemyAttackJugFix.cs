using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
using static EnemyMove;

public class EnemyAttackJugFix : MonoBehaviour
{

    //All null attributes will be ignored.
    [Header("Birth")]
    [Tooltip("The object to spawn when the enemy spawns")]
    public GameObject CreateOnStart;
    [Tooltip("The time to wait to spawn when the enemy spawns")]
    public float CreationDelay;

    [Tooltip("The recursive amount allowed to spawn when the enemy spawns")]
    public int CreationsAllowed;

    [Header("Death")]

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

    [Tooltip("If true, the enemy will only fire if they are at rest.")]
    public bool mustBeRestingToFire;
    [Tooltip("Will force bullets to be the child of the enemy.")]
    public bool bulletsAreChildren;

    [Tooltip("If true, causes the enemy to shoot upwards.")]
    public bool hasNoAim;

    [Tooltip("The randomized offset deviation range to which the enemy will shoot towards.")]
    public float aimRandomOffsetAngle;

    [Header("Collision")]


    [Tooltip("The damage dealt to the player on collision.")]
    public int damage = 2;

    [Tooltip("How often collision damage can be dealt to the player.")]
    public float attackDelay = 1f;


    GameObject player;

    private bool canDamage = true;
    private bool canShoot = true;

    Vector3 firingRot;

    float timeCreated;
    int shotsFired;
    private void Start()
    {
        timeCreated = Time.timeSinceLevelLoad;
        player = GameObject.FindGameObjectWithTag("Player");
        if (firingPoint == null)
        {
            firingPoint = transform;
        }
        if (CreationsAllowed > 0 && CreateOnStart != null)
        {
            StartCoroutine(DelayedCreate());
        }

        firingRot = firingPoint.localRotation.eulerAngles;
    }
    [HideInInspector]
    public bool hasCreatedStart;
    public IEnumerator DelayedCreate()
    {
        if (CreationDelay != 0)
            yield return new WaitForSeconds(CreationDelay);
        hasCreatedStart = true;
        if (CreationsAllowed > 0 && Vector2.Distance(transform.position, player.transform.position + new Vector3(0, 0.75f, 0)) > 0.1f && HasClearShot())
        {
            var tempRot = Quaternion.identity;

            if (!hasNoAim)
            {
                tempRot = Quaternion.LookRotation(player.transform.position - transform.position);
            }
            else
            {
                tempRot = Quaternion.Euler(firingRot.x, firingRot.y, firingRot.z + Random.Range(-aimRandomOffsetAngle, aimRandomOffsetAngle));
            }
            GameObject g;
            if (bulletsAreChildren)
            {
                g = Instantiate(CreateOnStart, firingPoint.position, firingPoint.rotation, firingPoint);
            }
            else
                g = Instantiate(CreateOnStart, firingPoint.position, firingPoint.rotation, transform.parent);


            if (g.GetComponent<EnemyAttack>() != null)
            {
                g.GetComponent<EnemyAttack>().CreationsAllowed = CreationsAllowed - 1;
            }

            if (g.GetComponent<DestroyMeOverTime>() != null && GetComponent<DestroyMeOverTime>() != null)
            {
                g.GetComponent<DestroyMeOverTime>().DelayTime = Mathf.Max(0, (GetComponent<DestroyMeOverTime>().DelayTime * 0.99f) - (Time.timeSinceLevelLoad - timeCreated));

                if (g.GetComponent<DestroyMeOverTime>().DelayTime <= 0)
                {
                    g.GetComponent<EnemyAttack>().CreationsAllowed = 0;
                }
            }

            firingPoint.localRotation = tempRot;
        }

    }

    public bool HasClearShot()
    {
        Vector2 pos = player.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (pos - new Vector2(transform.position.x, transform.position.y)).normalized, Vector2.Distance(transform.position, pos), ~LayerMask.GetMask("Enemy", "Player", "ForceField"));

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
                if (!mustBeRestingToFire || GetComponent<Rigidbody2D>().velocity.sqrMagnitude < 0.25f)
                {
                    var tempRot = Quaternion.identity;

                    if (!hasNoAim)
                    {
                        tempRot = Quaternion.LookRotation(player.transform.position - transform.position);
                    }
                    else
                    {
                        tempRot = Quaternion.Euler(firingRot.x, firingRot.y, firingRot.z + Random.Range(-aimRandomOffsetAngle, aimRandomOffsetAngle));
                    }
                    if (bulletsAreChildren)
                    {
                        Instantiate(bulletPrefab, firingPoint.position, firingPoint.rotation, firingPoint);
                    }
                    else
                        Instantiate(bulletPrefab, firingPoint.position, firingPoint.rotation, transform.parent);


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


                    firingPoint.localRotation = tempRot;
                }
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
