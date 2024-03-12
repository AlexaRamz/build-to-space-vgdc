using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Rigidbody2D rb;
    private float mx;
    private float my;

    // Weapon Variables
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPoint;
    [Range(0.1f,1f)]
    [SerializeField] private float firingRate = 0.5f;
    private float fireTimer;
    Vector2 mousePos;
    float angle;

    private SpriteRenderer spriteRender;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRender = GetComponent<SpriteRenderer>();
    }

    private void Shoot()
    {
        mx = Input.GetAxisRaw("Horizontal");
        my = Input.GetAxisRaw("Vertical");

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        angle = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0, 0, angle);

        Instantiate(bulletPrefab, firingPoint.position, firingPoint.rotation);
    }

    void Update()
    {
        // Gun Rotation
        Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = rotation;

        if(angle < 89 && angle > -89)
        {
            spriteRender.flipY = false;
        }
        else
        {
            spriteRender.flipY = true;
        }

        // Shooting Cooldown Timer
        if(Input.GetMouseButton(0) && fireTimer <= 0f)
        {
            Shoot();
            fireTimer = firingRate;
        }
        else
        {
            fireTimer -= Time.deltaTime;
        }
    }
}
