using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tube : MonoBehaviour
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight,
        LeftUp,
        LeftDown,
        RightUp,
        RightDown,
    }
    public Direction direction = Direction.Up;
    float speed = 3f;

    void SetDirection(Collider2D col)
    {
        Vector2 velocity = Vector2.zero;
        if (direction == Direction.Up)
        {
            velocity = new Vector2(0, speed);
        }
        else if (direction == Direction.Down)
        {
            velocity = new Vector2(0, -speed);
        }
        else if (direction == Direction.Left)
        {
            velocity = new Vector2(-speed, 0);
        }
        else if (direction == Direction.Right)
        {
            velocity = new Vector2(speed, 0);
        }
        else if (direction == Direction.UpLeft)
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
        else if (direction == Direction.LeftDown)
        {
            if (col.bounds.center.x > transform.position.x)
            {
                velocity = new Vector2(-speed, 0);
            }
            else
            {
                velocity = new Vector2(0, -speed);
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
