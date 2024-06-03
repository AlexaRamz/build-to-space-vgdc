using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class forcefield : MonoBehaviour
{
    private CircleCollider2D forcefieldCollider;
    private SpriteRenderer forcefieldSprite;
    public KeyCode activationKey = KeyCode.F;
    public float bounceForce = 5f;


    // Start is called before the first frame update
    void Start()
    {
        forcefieldCollider = GetComponent<CircleCollider2D>();
        forcefieldSprite = GetComponent<SpriteRenderer>();

        forcefieldCollider.enabled = false;
        forcefieldSprite.enabled = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(activationKey))
        {
            forcefieldCollider.enabled = !forcefieldCollider.enabled;
            forcefieldSprite.enabled = !forcefieldSprite.enabled;
        }
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        Rigidbody2D objectRb = other.gameObject.GetComponent<Rigidbody2D>();

        if (objectRb != null)
        {
            Vector2 bounceDirection = (other.transform.position - transform.position).normalized;
            objectRb.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);
        }
    }
}
