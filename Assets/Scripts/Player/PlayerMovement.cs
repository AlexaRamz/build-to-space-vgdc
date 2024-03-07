using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;
    public bool plrActive = true;
    private Rigidbody2D rb;
    private Animator anim;
    private float runSpeed = 7f;
    private float jumpSpeed = 7f;
    private bool isGrounded = true;
    private float jumpTime = 0.25f;
    private bool inTube = false;
    [SerializeField] 
    private GameObject smokePuffsPrefab;

    // Jetpack
    [SerializeField] private GameObject jetPack;
    private bool jetPackOn = false;
    private float jetForce;
    [SerializeField]
    private Slider fuelSlider;
    [SerializeField]
    private float fuel = 100f;
    [SerializeField]
    private float fuelBurnRate = 30f;
    [SerializeField]
    private float fuelRefillRate = 5f;
    private float currentFuel;
    private bool haveFuel = true;
    private float refillCoolDown = 2f;
    private float fuelTimer;

    private void Start()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        jetForce = rb.gravityScale * 9f * 2f;
        currentFuel = fuel;
    }
    void PlayCrashParticles()
    {
        GameObject smokePuffs = Instantiate(smokePuffsPrefab, transform.position, Quaternion.Euler(-90, 0, 0));
        smokePuffs.GetComponent<ParticleSystem>().Play();
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

            if (!inTube && !jetPackOn)
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

            /* JETPACK */
            if (haveFuel && Input.GetKey("space"))
            {
                jetPackOn = true;
                fuelSlider.gameObject.SetActive(true);
                anim.SetBool("Jumping", true);
            }
            else
            {
                jetPackOn = false;
                RefillFuel();
            }
            fuelSlider.value = currentFuel / fuel;
            jetPack.GetComponent<Animator>().SetBool("Fly", jetPackOn);
            if (currentFuel < 0)
            {
                haveFuel = false;
                currentFuel = 0;
            }
            if (!haveFuel)
            {
                fuelTimer += Time.deltaTime;
                if (fuelTimer >= refillCoolDown)
                {
                    haveFuel = true;
                    fuelTimer = 0;
                } 
            }
        }
    }

    private void FixedUpdate()
    {
        if (jetPackOn)
        {
            rb.AddForce(Vector2.up * jetForce);
            currentFuel -= fuelBurnRate * Time.deltaTime;
        }
    }

    void RefillFuel()
    {
        if (currentFuel < fuel)
        {
            currentFuel += fuelRefillRate * Time.deltaTime;
        }
        else
        {
            fuelSlider.gameObject.SetActive(false);
        }
    }
}
