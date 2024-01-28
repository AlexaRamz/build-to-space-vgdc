using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool plrActive = true;
    Rigidbody2D rb;
    Animator anim;
    float runSpeed = 7f;
    float jumpSpeed = 7f;
    bool isGrounded = true;
    public float jumpTime = 0.25f;
    public bool inTube = false;

    public GameObject jetPack;
    bool jetPackOn = false;
    public float jetForce = 2f;
    public ParticleSystem smokeParticles;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    void PlayCrashParticles()
    {
        smokeParticles.transform.position = transform.position;
        smokeParticles.Stop();
        smokeParticles.Play();
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.isTrigger == false && col.gameObject.tag != "Collectable" && col.gameObject.tag != "Player" && !inTube)
        {
            isGrounded = true;
            if (rb.velocity.y < -10f)
            {
                PlayCrashParticles();
            }
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.isTrigger == false && col.gameObject.tag != "Collectable" && col.gameObject.tag != "Player" && !inTube)
        {
            isGrounded = false;
        }
    }
    
    public void HoldAnim()
    {
        anim.SetBool("Holding", true);
    }
    public void CancelHoldAnim()
    {
        anim.SetBool("Holding", false);
    }
    public void InTube()
    {
        inTube = true;
        canJump = false;
    }
    public void OutTube()
    {
        inTube = false;
    }

    bool debounce = false;
    IEnumerator JumpDelay()
    {
        yield return new WaitForSeconds(.02f);
        debounce = false;
    }
    bool wasMoving;
    float jumpTimeCounter;
    bool jumpButtonDown = false;
    bool canJump = false;
    void Update()
    {
        if (plrActive)
        {
            float dirX = Input.GetAxisRaw("Horizontal");

            if (dirX == 0)
            {
                if (isGrounded)
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
                else if (wasMoving)
                {
                    wasMoving = false;
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
            }
            else
            {
                rb.velocity = new Vector2(dirX * runSpeed, rb.velocity.y);
                wasMoving = true;
            }

            if (dirX > 0f)
            {
                anim.SetBool("RunningRight", true);
                anim.SetBool("RunningLeft", false);
            }
            else if (dirX < 0f)
            {
                anim.SetBool("RunningRight", false);
                anim.SetBool("RunningLeft", true);
            }
            else
            {
                anim.SetBool("RunningRight", false);
                anim.SetBool("RunningLeft", false);
            }
            if (Input.GetKeyDown("up"))
            {
                jumpButtonDown = true;
            }
            if (Input.GetKeyUp("up"))
            {
                jumpButtonDown = false;
                canJump = false;
            }

            if (!inTube)
            {
                //Jumping
                if (isGrounded)
                {
                    if (jumpButtonDown)
                    {
                        if (debounce == false)
                        {
                            debounce = true;
                            canJump = true;
                            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                            jumpTimeCounter = 0;
                            StartCoroutine(JumpDelay());
                        }
                    }
                    else
                    {
                        anim.SetBool("Jumping", false);
                    }
                }
                else
                {
                    anim.SetBool("Jumping", true);
                    if (canJump && jumpButtonDown && jumpTimeCounter < jumpTime)
                    {
                        rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                        jumpTimeCounter += Time.deltaTime;
                    }
                }
            }
            else
            {
                //Up and down moving

            }
            // Jetpack
            if (Input.GetKeyDown("space"))
            {
                jetPack.GetComponent<Animator>().SetBool("Fly", true);
                jetPackOn = true;
            }
            else if (Input.GetKeyUp("space"))
            {
                jetPack.GetComponent<Animator>().SetBool("Fly", false);
                jetPackOn = false;
            }
            if (jetPackOn)
            {
                rb.AddForce(Vector2.up * jetForce);
                //if (isGrounded)
                //{
                    //smokeParticles.Play();
                //}
            }
        }
    }
}
