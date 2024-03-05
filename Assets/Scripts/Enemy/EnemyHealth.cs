using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
       
    }
    public void Die()
    {
        if(GetComponent<EnemyAttack>()!= null)
        {
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
