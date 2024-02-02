using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    Rigidbody2D rb;
    public float moveSpeed = 2f;

    public enum EnemyState
    {
        Wander,
        Patrol,
        Chase,
    }
    public EnemyState moveState;
    [SerializeField]
    private bool fly = false;

    // Wander
    public float moveTimeMin = 3;
    public float moveTimeMax = 5;
    public float idleTimeMin = 2;
    public float idleTimeMax = 4;

    // Patrol
    public Transform patrolPointA;
    public Transform patrolPointB;
    Transform currentPoint;

    // Chase
    public Transform target;

    bool arrived = true;
    float arriveThreshold = 0.1f;
    Vector2 destination;
    bool hasDestination = false;
    IEnumerator currentCoroutine;

    // Edge and Wall check
    Transform wallTrigger;
    Transform groundCheck;
    float edgeDistance;
    float wallDistance;
    public bool avoidEdges = true;
    
    public float detectionRange = 5f;
    bool playerDetected = false;
    public bool lockFacing = false;
    public bool lockState = false;

    SpriteRenderer sr;
    Animator anim;
    Transform spriteTransform;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteTransform = transform.Find("Sprite");
        sr = spriteTransform.GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        currentPoint = patrolPointA;
        wallTrigger = transform.Find("WallTrigger");
        groundCheck = transform.Find("GroundCheck");
        wallDistance = wallTrigger.localPosition.x;
        edgeDistance = groundCheck.localPosition.x;
        if (fly)
        {
            rb.gravityScale = 0;
        }
    }

    int RandomSign()
    {
        if (Random.Range(0, 2) == 0)
        {
            return -1;
        }
        return 1;
    }
    IEnumerator WanderX_C()
    {
        int randDirX = RandomSign();
        SetFacing(new Vector2(randDirX, 0));
        rb.velocity = new Vector2(randDirX * moveSpeed, rb.velocity.y);
        yield return new WaitForSeconds(Random.Range(moveTimeMin, moveTimeMax));
        rb.velocity = new Vector2(0, rb.velocity.y);
        yield return new WaitForSeconds(Random.Range(idleTimeMin, idleTimeMax));
        arrived = true;
    }
    void WanderX()
    {
        currentCoroutine = WanderX_C();
        StartCoroutine(currentCoroutine);
    }

    IEnumerator Wander_C()
    {
        Vector2 randDir = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized;
        SetFacing(randDir);
        rb.velocity = randDir * moveSpeed;
        yield return new WaitForSeconds(Random.Range(moveTimeMin, moveTimeMax));
        rb.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(Random.Range(idleTimeMin, idleTimeMax));
        arrived = true;
    }
    void Wander()
    {
        currentCoroutine = Wander_C();
        StartCoroutine(currentCoroutine);
    }

    void SetFacing(Vector2 dir)
    {
        if (!lockFacing)
        {
            float dirSign = Mathf.Sign(dir.x);
            sr.flipX = dirSign < 0;

            Vector3 localPos = wallTrigger.localPosition;
            localPos.x = dirSign * wallDistance;
            wallTrigger.localPosition = localPos;

            localPos = groundCheck.localPosition;
            localPos.x = dirSign * edgeDistance;
            groundCheck.localPosition = localPos;
        }
    }

    void ChangeState(EnemyState newState)
    {
        moveState = newState;
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        arrived = true;
    }
    void SwitchDirection()
    {
        if (moveState == EnemyState.Wander)
        {
            Vector2 newDir = new Vector2(-rb.velocity.x, rb.velocity.y);
            SetFacing(newDir);
            rb.velocity = newDir;
        }
    }
    public void WallTriggerEnter() // Change direction before a wall
    {
        SwitchDirection();
    }
    public void GroundCheckExit() // Change direction before an edge
    {
        if (avoidEdges)
        {
            SwitchDirection();
        }
    }

    float rotateSpeed = 100f;
    void Animate()
    {
        if (!fly)
        {
            anim.SetBool("Walk", Mathf.Abs(rb.velocity.x) > 0.001f);
        }
        else
        {
            if (Mathf.Abs(rb.velocity.x) > 0.001f)
            {
                float angle = Mathf.Sign(rb.velocity.x) * Mathf.Sign(rb.velocity.y) * 20f;
                Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    void Update()
    {
        if (arrived == true)
        {
            switch (moveState)
            {
                case EnemyState.Wander:
                    arrived = hasDestination = false;
                    if (fly)
                    {
                        Wander();
                    }
                    else
                    {
                        WanderX();
                    }
                    break;
                case EnemyState.Patrol:
                    // Move between back and forth between two points
                    if (patrolPointA && patrolPointB)
                    {
                        if (currentPoint == patrolPointA)
                        {
                            currentPoint = patrolPointB;
                        }
                        else
                        {
                            currentPoint = patrolPointA;
                        }
                        arrived = false;
                        hasDestination = true;
                        destination = currentPoint.position;
                    }
                    break;
                case EnemyState.Chase:
                    if (target)
                    {
                        hasDestination = true;
                        destination = target.position;
                    }
                    break;
            }
        }
        if (hasDestination)
        {
            if (fly)
            {
                if (Vector2.Distance(transform.position, destination) < arriveThreshold)
                {
                    arrived = true;
                    rb.velocity = new Vector2(0, 0);
                }
                else
                {
                    Vector2 dir = (destination - (Vector2)transform.position).normalized;
                    SetFacing(dir);
                    rb.velocity = dir * moveSpeed;
                }
            }
            else
            {
                if (Mathf.Abs(transform.position.x - destination.x) < arriveThreshold)
                {
                    arrived = true;
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
                else
                {
                    float dirX = Mathf.Sign(destination.x - transform.position.x);
                    SetFacing(new Vector2(dirX, 0));
                    rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
                }
            }
        }
        if (!lockState)
        {
            Collider2D col = Physics2D.OverlapCircle(transform.position, detectionRange, LayerMask.GetMask("Player"));
            if (col != null)
            {
                if (!playerDetected)
                {
                    playerDetected = true;
                    target = col.transform;
                    ChangeState(EnemyState.Chase);
                }
            }
            else if (playerDetected)
            {
                playerDetected = false;
                ChangeState(EnemyState.Wander);
            }
        }
        Animate();
    }
}
