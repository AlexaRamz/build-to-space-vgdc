using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool plrActive = true;
    private Rigidbody2D rb;
    private Animator anim;
    private float runSpeed = 7f;
    private float jumpSpeed = 7f;
    private bool isGrounded = true;
    public float jumpTime = 0.25f;
    public bool inTube = false;

    public GameObject jetPack;
    private bool jetPackOn = false;
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
        if (!col.isTrigger && col.gameObject.tag != "Collectable" && col.gameObject.tag != "Player" && !inTube)
        {
            isGrounded = true;
            if (rb.velocity.y < -10f) //Play dust particles if player lands on the ground heavily
            {
                PlayCrashParticles();
            }
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (!col.isTrigger && col.gameObject.tag != "Collectable" && col.gameObject.tag != "Player" && !inTube)
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

            anim.SetBool("RunningRight", dirX > 0f);
            anim.SetBool("RunningLeft", dirX < 0f);

            jumpButtonDown = Input.GetKey("up") || Input.GetKey("w");
            if(!jumpButtonDown)
                canJump = false;

            if (!inTube)
            {
                //Jumping
                if (isGrounded)
                {
                    if (jumpButtonDown)
                    {
                        if (!debounce)
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
            jetPackOn = Input.GetKey("space");
            jetPack.GetComponent<Animator>().SetBool("Fly", jetPackOn);
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
