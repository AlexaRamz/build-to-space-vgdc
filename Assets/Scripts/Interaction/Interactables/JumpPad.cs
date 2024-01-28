using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float bounce = 10f;
    bool Debounce = false;
    float debounceTime = 0.01f;

    IEnumerator DebounceDelay()
    {
        yield return new WaitForSeconds(debounceTime);
        Debounce = false;
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!Debounce && col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("Collectable"))
        {
            Debounce = true;
            col.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounce, ForceMode2D.Impulse);
            StartCoroutine(DebounceDelay());
        }
    }
}
