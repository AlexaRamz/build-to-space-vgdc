using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    public int maxHealth = 10;
    private Vector3 spawnPos;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        spawnPos = transform.position + new Vector3(0, 0.5f, 0);
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log("hit");
        if (health <= 0)
        {
            //Destroy(gameObject);
            transform.position = spawnPos;
            health = maxHealth;
        }
    }
}
