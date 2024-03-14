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
    private bool holding;

    Inventory plrInv;

    private void Start()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        jetForce = rb.gravityScale * 9f * 2f;
        currentFuel = fuel;
        plrInv = GetComponent<Inventory>();
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
        holding = true;
    }
    public void CancelHoldAnim()
    {
        anim.SetBool("Holding", false);
        holding = false;
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
    bool jumpButton = false;
    bool canJump = false;

    int faceDirection = 1;

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
                if (!holding)
                {
                    if (dirX > 0) faceDirection = 1;
                    else faceDirection = -1;
                }
            }

            if (holding)
            {
                float distance = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x;
                if (Mathf.Abs(distance) > 0.25f)
                {
                    if (distance > 0) faceDirection = 1;
                    else faceDirection = -1;
                    plrInv.holdOrigin.localScale = new Vector3(1, faceDirection, 1);
                }
            }
            anim.SetFloat("Horizontal", faceDirection);
            anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

            jumpButton = Input.GetKey("up") || Input.GetKey("w");
            if (!jumpButton) canJump = false;

            if (!inTube && !jetPackOn)
            {
                //Jumping
                if (isGrounded)
                {
                    if (Input.GetKeyDown("up") || Input.GetKeyDown("w"))
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
                    if (canJump && jumpButton && jumpTimeCounter < jumpTime)
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
        else // player not active
        {
            if (Input.GetKey("up") || Input.GetKey("w") || Input.GetKey("down") || Input.GetKey("s"))
            {
                Unsit();
            }
            if (sitting)
            {
                transform.position = seat.seatPoint.position;
                transform.rotation = seat.seatPoint.rotation;
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

    bool sitting;
    Seat seat;
    public void Sit(Seat thisSeat)
    {
        if (seat) Unsit();
        sitting = true;
        seat = thisSeat;
        seat.beingUsed = true;
        plrActive = false;
        rb.gravityScale = 0;
    }

    public void Unsit()
    {
        sitting = false;
        seat.beingUsed = false;
        seat = null;
        plrActive = true;
        rb.gravityScale = 1.5f;
        transform.rotation = Quaternion.identity;
    }
}
