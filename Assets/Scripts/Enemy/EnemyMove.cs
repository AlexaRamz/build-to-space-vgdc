using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class EnemyMove : MonoBehaviour
{


    [Header("Movement Attributes")]
    [Tooltip("The patrol path that the enemy will be taking.")]
    public List<PathNode> pathNodes; //This may be empty if our enemy is not going to be patroling.
    
    private PathNode tempTarget=null;//This is our current target that we will work towards getting to.
    [Tooltip("The current AI movement state of the enemy (This changes as the game runs)")]

    public enum MoveState {Idle,Wander,Patrol,Chase}
    public MoveState moveState;
    public enum MovementFlare {Walk,Fly,Hop}
    [Tooltip("The movement type of the enemy. (This does not factor in gravity in the rigidbody!)")]
    public MovementFlare moveType;//True if this enemy should not experience gravity.
    SpriteRenderer spriteRenderer;
    Rigidbody2D rig;
    GameObject player;
    Vector2 spawnPosition;

    private Coroutine last_idlewait;
    [Min(0),Tooltip("How fast the enemy moves towards its targets. (Remember to factor in Linear Drag in the rigidbody)")]
    public float Speed=1;
    [Header("Data Attributes")]
    [Min(0),Tooltip("How close the enemy will try to get to its movement targets. (Larger values have more rounded trajectories)")]
    public float minArriveDist = 0.5f;//How close the enemy has to get to be said to "arrive" somewhere
    [Min(0),Tooltip("The lower bound for how long the enemy will wait after completing a wander target before wandering again")]
    public float minWaitWander=1;

    [Min(0), Tooltip("The upper bound for how long the enemy will wait after completing a wander target before wandering again")]
    public float maxWaitWander=2;

    [Min(0), Tooltip("The lower bound for how long the enemy will wait after completing a patrol target before moving on")]
    public float minWaitPatrol=1;

    [Min(0), Tooltip("The upper bound for how long the enemy will wait after completing a patrol target before moving on")]
    public float maxWaitPatrol=2;


    [Min(0),Tooltip("The maximum distance that the enemy will try to wander too")]
    public float maxWanderDistance = 10;
    [Min(0),Tooltip("The distance in which the enemy will detect the player and begin chasing")]
    public float maxChaseDetectDistance = 2;

    [Min(0),Tooltip("The power in which the enemy hops upwards, does not apply if the enemy is not hopping")]
    public float hopPower=16;
    private float hopGroundDist = 0.8f;

    [System.Serializable]
    public class PathNode 
    {
        //This class if for our patrols, where we will want to have a set of locations that are our patrol route.
        public PathNode()
        {

        }
        public PathNode(Vector2 pos)
        {
            Position = pos;
        }
        public PathNode(GameObject targ)
        {
            SnapToTarget = targ;
            Position= targ.transform.position;  
        }

        public Vector2 Position;

        [Header("Override Position To Target:")]
        public GameObject SnapToTarget;
        public Vector2 getPos()
        {
            if(SnapToTarget!=null)
            {
                return SnapToTarget.transform.position+new Vector3(0,0.75f,0);//Offset to center of sprite
            }
            else 
            { 
                return Position; 
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (pathNodes==null)
        {
            pathNodes=new List<PathNode>();
        }

        if (pathNodes.Count > 0)
        {
            Vector2 lastPos = pathNodes[pathNodes.Count - 1].getPos();
            foreach (PathNode node in pathNodes)
            {
                if (node.SnapToTarget != null)
                {
                    node.Position = node.SnapToTarget.transform.position;
                }
                Gizmos.DrawLine(lastPos, node.getPos());
                lastPos = node.Position;
            }
        }
        Gizmos.color = Color.green;
        if(tempTarget!=null)
        {
            Gizmos.DrawLine(transform.position, tempTarget.getPos());
        }
    }
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rig = GetComponent<Rigidbody2D>();
        spawnPosition = transform.position;
        tempTarget = null;
        //Replace with a better way to locate the player
        player = GameObject.FindGameObjectWithTag("Player");
        //
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
            case MoveState.Idle:
                Idle();
                break;
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
    private IEnumerator WaitIdle(float duration,MoveState nextState) //Waits idle for duration seconds and then sets the movestate to nextState
    {
        moveState = MoveState.Idle;
        yield return new WaitForSeconds(duration);
        moveState = nextState;
        last_idlewait = null;
    }
    public bool isCurrentlyWaitingIdle() //Returns true if this enemy is currenlty waiting in WaitIdle()
    {
        return last_idlewait != null;
    }
    public void WaitIdleForABit(float duration,MoveState nextState) //Starts a WaitIdle Coroutine, overrides preexisting WaitIdles
    {
        if(isCurrentlyWaitingIdle())
        {
            StopCoroutine(last_idlewait);
        }
        last_idlewait = StartCoroutine(WaitIdle(duration, nextState));
    }
    public void AbortIdleWait()//Aborts the current WaitIdle Coroutine if applicable.
    {
        if (isCurrentlyWaitingIdle())
        {
            StopCoroutine(last_idlewait);
            last_idlewait = null;
        }
    }

    public void Animate() //Updates the Animation for the Enemey based on movement.
    {
        //Replace this with enemy animations \
        if (rig.velocity.sqrMagnitude > 0.25f)
        {
            spriteRenderer.flipX = rig.velocity.x > 0;
        }
        //Replace this with enemy animations /
    }
    public bool CheckShouldChase() //Snaps the Enemy into a chase state if it should chase, returns true/false depending on whether or not chasing should be occuring now.
    {

        if(Vector2.Distance(transform.position,player.transform.position) > maxChaseDetectDistance || !CanReachPosition(new PathNode(player)))
        {

            //I did not find the player, and I am chasing, cancel time!
            if (moveState == MoveState.Chase)
            {
                moveState = MoveState.Idle;
                tempTarget = null;
                WaitIdleForABit(1, MoveState.Idle);
                
            }
            return false;
        }
        //I found the player, chase time!
        if (moveState != MoveState.Chase)
        {
            AbortIdleWait();
            moveState = MoveState.Chase;
            tempTarget=new PathNode(player);
        }
        return true;
    }
    public void Idle()
    {
        if (CheckShouldChase())
            return;

        if(!isCurrentlyWaitingIdle())
        {
            //The wait timer has expired, so we should exit Idle and enter Wander/Patrol now.
            if(pathNodes.Count== 0)
            {
                moveState = MoveState.Wander;
            }
            else
            {
                moveState = MoveState.Patrol;
            }
        }
    }
    public bool CanReachPosition(PathNode pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (pos.getPos()-new Vector2(transform.position.x,transform.position.y)).normalized,Vector2.Distance(transform.position,pos.getPos()), ~LayerMask.GetMask("NPC", "Player"));

        if (hit.collider != null)
        {
            //Something is in the way!
            return false;
        }
        return true;
    }
    public PathNode genNextWanderPos()
    {
       for (int i = 0; i < 10; i++)
       {
            PathNode pos=null;
            if(moveType!=MovementFlare.Fly)
            {
                Vector2 targ = new Vector2(transform.position.x + Random.Range(-maxWanderDistance / 2, maxWanderDistance / 2), transform.position.y );

                if (Vector2.Distance(transform.position, spawnPosition) > maxWanderDistance)
                {
                    targ = new Vector2(spawnPosition.x + Random.Range(-maxWanderDistance / 2, maxWanderDistance / 2), spawnPosition.y );
                }

                RaycastHit2D hit = Physics2D.Raycast(targ,Vector2.down,float.MaxValue, ~LayerMask.GetMask("NPC", "Player"));

                if (hit.collider == null)
                {
                    return null;
                }
                else
                {
                    pos = new PathNode(hit.point + new Vector2(0, 0.75f));
                }
            }
            else
            {
                Vector2 targ = new Vector2(transform.position.x + Random.Range(-maxWanderDistance / 2, maxWanderDistance / 2), transform.position.y + Random.Range(-maxWanderDistance / 2, maxWanderDistance / 2));

                if (Vector2.Distance(transform.position, spawnPosition) > maxWanderDistance)
                {
                    targ = new Vector2(spawnPosition.x + Random.Range(-maxWanderDistance / 2, maxWanderDistance / 2), spawnPosition.y + Random.Range(-maxWanderDistance / 2, maxWanderDistance / 2));
                }

                pos = new PathNode(targ);
            }

            if (Vector2.Distance(spawnPosition,pos.getPos())< maxWanderDistance && CanReachPosition(pos))
            {
                return pos;
            }
       }
       return null;
    }
    public void Wander()
    {
        if (CheckShouldChase())
            return;


        if(tempTarget==null||Vector2.Distance(transform.position,tempTarget.getPos())<=minArriveDist)
        {
            //Find me a new target location to wander towards!
            tempTarget = genNextWanderPos();



            if(tempTarget!=null)
                WaitIdleForABit(Random.Range(minWaitWander, maxWaitWander), MoveState.Wander);
        }
        else
        {//tempTarget != null and distance > 1f

            //Move me towards the target location!
            Vector2 dir = (tempTarget.getPos()-new Vector2(transform.position.x,transform.position.y)).normalized;

            if(moveType==MovementFlare.Hop)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, float.MaxValue, ~LayerMask.GetMask("NPC", "Player"));

                if (hit.collider == null)
                {
                    return; //Im over the void, so lets just give up on chasing for this run...
                }
                else
                {
                    if(Vector2.Distance(hit.point,transform.position)< hopGroundDist && Mathf.Abs(rig.velocity.y)<0.01f)
                    {//Hopping movement is only allowed when I am on the ground, and not moving downwards.

                        dir.y += 1;
                        rig.AddForce(new Vector2(dir.x * Speed, dir.y * hopPower), ForceMode2D.Impulse);
                    }
                }
            }
            else
            {
                rig.AddForce(dir * Speed, ForceMode2D.Force);
            }


        }
    }

    public void Patrol()
    {
        if (CheckShouldChase())
            return;

        //We can assume pathNodes is not empty if we are here. (Error elsewise)

        if (tempTarget == null || Vector2.Distance(transform.position, tempTarget.getPos()) <= minArriveDist)
        {
            //Take the next pathNode and set it as my target, then shift it to the bottom of the list.
            tempTarget = pathNodes[0];
            pathNodes.RemoveAt(0);
            pathNodes.Add(tempTarget);
            WaitIdleForABit(Random.Range(minWaitPatrol, maxWaitPatrol), MoveState.Patrol);
        }
        else
        {//tempTarget != null and distance > 1f

            //Move me towards the target location!
            Vector2 dir = (tempTarget.getPos() - new Vector2(transform.position.x, transform.position.y)).normalized;

            if (moveType == MovementFlare.Hop)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, float.MaxValue, ~LayerMask.GetMask("NPC", "Player"));

                if (hit.collider == null)
                {
                    return; //Im over the void, so lets just give up on chasing for this run...
                }
                else
                {
                    if (Vector2.Distance(hit.point, transform.position) < hopGroundDist && Mathf.Abs(rig.velocity.y) < 0.01f)
                    {//Hopping movement is only allowed when I am on the ground, and not moving downwards.

                        dir.y += 1;
                        rig.AddForce(new Vector2(dir.x * Speed, dir.y * hopPower), ForceMode2D.Impulse);
                    }
                }
            }
            else
            {
                rig.AddForce(dir * Speed, ForceMode2D.Force);
            }

        }
    }

    public void Chase()
    {

        if (!CheckShouldChase())
            return;

        if(tempTarget==null)
        {
            tempTarget = new PathNode(player);
        }

        if (Vector2.Distance(tempTarget.getPos(), transform.position) > 1) //Dont get too close, we just want them to hit the player
        {
            Vector2 targPos = tempTarget.getPos();
            if(moveType!=MovementFlare.Fly)
            {
                //We shouldnt fly, so lets map our targ pos downwards to the ground.
                RaycastHit2D hit = Physics2D.Raycast(targPos, Vector2.down, float.MaxValue, ~LayerMask.GetMask("NPC", "Player"));

                if (hit.collider == null)
                {
                    return; //Im over the void, so lets just give up on chasing for this run...
                }
                else
                {
                    targPos = (hit.point + new Vector2(0, 0.5f));
                }
            }
            Vector2 dir = (targPos - new Vector2(transform.position.x, transform.position.y)).normalized;

            if (moveType == MovementFlare.Hop)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, float.MaxValue, ~LayerMask.GetMask("NPC", "Player"));

                if (hit.collider == null)
                {
                    return; //Im over the void, so lets just give up on chasing for this run...
                }
                else
                {
                    if (Vector2.Distance(hit.point, transform.position) < hopGroundDist && Mathf.Abs(rig.velocity.y) < 0.01f)
                    {//Hopping movement is only allowed when I am on the ground, and not moving downwards.

                        dir.y += 1;
                        rig.AddForce(new Vector2(dir.x*Speed,dir.y*hopPower), ForceMode2D.Impulse);
                    }
                }
            }
            else
            {
                rig.AddForce(dir * Speed, ForceMode2D.Force);
            }
        }
    }

}
