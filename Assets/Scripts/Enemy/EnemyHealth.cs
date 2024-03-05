using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyMove;
using static UnityEditor.PlayerSettings;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 15;
    public int currentHealth; 

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        if(GetComponent<EnemyAttack>()!= null)
        {
            if (GetComponent<EnemyAttack>().snapOnDeathCreationToGround)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, float.MaxValue, ~LayerMask.GetMask("NPC", "Player"));

                if (hit.collider != null)
                { 
                    Instantiate(GetComponent<EnemyAttack>().createOnDeath, hit.point+new Vector2(0,0.75f), transform.rotation, transform.parent);
                }
            }
            else
                Instantiate(GetComponent<EnemyAttack>().createOnDeath, transform.position, transform.rotation, transform.parent);
        }
        Destroy(gameObject);
    }
    public void TakeDamage(int amount)
    {
        currentHealth -= amount; 
        if(currentHealth <= 0)
        {
            Die();
        }
    }

}
