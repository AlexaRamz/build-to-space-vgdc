using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 15;
    public int currentHealth;
    [Tooltip("The object to spawn when the enemy dies.")]
    public GameObject createOnDeath;
    [Tooltip("Requires the death object to only spawn on the ground.")]
    public bool snapOnDeathCreationToGround;
    [SerializeField] QuestManager questManager;

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
        if (createOnDeath != null)
        {
            if (snapOnDeathCreationToGround)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, float.MaxValue, ~LayerMask.GetMask("NPC", "Player"));
                if (hit.collider != null)
                { 
                    Instantiate(createOnDeath, hit.point+new Vector2(0,0.05f), transform.rotation, transform.parent);
                }
            }
            else
                Instantiate(createOnDeath, transform.position, transform.rotation, transform.parent);
        }
        questManager.UpdateHuntQuests(gameObject.name);
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
