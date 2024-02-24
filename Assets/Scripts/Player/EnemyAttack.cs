using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int damage = 2;
    public float attackDelay = 1f;
    private bool canAttack = true;

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (canAttack && collision.gameObject.tag == "Player")
        {
            canAttack = false;
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
            StartCoroutine(AttackDelay());
        }
    }
}
