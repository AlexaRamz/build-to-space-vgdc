using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [System.Serializable]
    public class PathNode 
    {
        //This class if for our patrols, where we will want to have a set of locations that are our patrol route.

        [Header("SnapToTarget will override the position with the target gameobject's location.")]
        public GameObject SnapToTarget;

        public Vector2 Position;
    }

    public enum MoveState
    {
        Wander,
        Patrol,
        Chase
    }
    private void OnDrawGizmosSelected()
    {
        Vector2 lastPos= transform.position;
        Gizmos.color = Color.yellow;
        if (pathNodes==null)
        {
            pathNodes=new List<PathNode>();
        }
        foreach(PathNode node in pathNodes)
        {
            Gizmos.DrawLine(lastPos,node.Position);
            if(node.SnapToTarget!=null)
            {
                node.Position=node.SnapToTarget.transform.position;
            }
            lastPos = node.Position;
        }
    }
    public List<PathNode> pathNodes; //This may be empty if our enemy is not going to be patroling.

    public MoveState moveState;
    public bool ShouldFly;//True if this enemy should not experience gravity.
    SpriteRenderer spriteRenderer;
    Rigidbody2D rig;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rig = GetComponent<Rigidbody2D>();
        if (pathNodes == null)
        {
            pathNodes = new List<PathNode>();
        }
    }

    void Update()
    {
        Animate();


        switch (moveState)
        {
            case MoveState.Wander:
                Wander();
                break;
            case MoveState.Patrol:
                Patrol();
                break;
            case MoveState.Chase:
                Chase();
                break;
        }
    }

    public void Animate()
    {
        //Replace this with enemy animations \
        spriteRenderer.flipX = rig.velocity.x > 0;
        //Replace this with enemy animations /
    }

    public void Wander()
    {
        Debug.LogWarning("Wandering has not been implemented yet!");
    }

    public void Patrol()
    {
        Debug.LogWarning("Patrols have not been implemented yet!");
    }

    public void Chase()
    {
        Debug.LogWarning("Chases have not been implemented yet!");
    }

}
