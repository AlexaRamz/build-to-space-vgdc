using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tube : MonoBehaviour
{
    public enum Direction
    {
        Straight,
        Corner,
    }
    public Direction direction;
    float speed = 3f;


    Vector2 AngleToVector2(float angleInDegrees)
    {
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

        // Calculate X and Y components using sine and cosine
        float x = Mathf.Cos(angleInRadians);
        float y = Mathf.Sin(angleInRadians);

        // Create a Vector2 representing the direction
        Vector2 directionVec = new Vector2(x, y);
        return directionVec;
    }
    void SetDirection(Collider2D col)
    {
        Vector2 velocity = Vector2.zero;
        if (direction == Direction.Straight)
        {
            float angleInDegrees = transform.localEulerAngles.z;
            velocity = AngleToVector2(angleInDegrees) * speed;
            Debug.Log(velocity);
        }
        else if (direction == Direction.Corner)
        {
            //Debug.Log(col);
            //Debug.Log(col.bounds.center.y - transform.position.y);
            if (col.bounds.center.y < transform.position.y)
            {
                velocity = new Vector2(0, speed);
            }
            else
            {
                velocity = new Vector2(-speed, 0);
            }
        }
        col.gameObject.GetComponent<Rigidbody2D>().velocity = velocity;
        col.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0f;
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Collectable")
        {
            SetDirection(col);
        }
        else if (col.gameObject.tag == "Player")
        {
            SetDirection(col);
            col.gameObject.GetComponent<PlayerMovement>().InTube();
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Collectable")
        {
            col.gameObject.GetComponent<Rigidbody2D>().gravityScale = 1.5f;
        }
        else if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<Rigidbody2D>().gravityScale = 1.5f;
            col.gameObject.GetComponent<PlayerMovement>().OutTube();
        }
    }
}
