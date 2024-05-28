using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{
    public bool orbitalAssist = false;
    private float gravityx;
    private float gravityy; //Records net gravity values
    private float c; //Used in gravity calculation
    private float currentMass = 0.1f; //Set to 1 as a default, used to record current rigid body mass
    private float planetMass = 2; //Set to 2 as default, used to record current planet mass
    public GameObject[] planets; //Records list of all planets in level
    public Rigidbody2D[] rigidBodies; //Records list of all rigid bodies in level
    float gravitationalConstant = 19.62f;

    // Start is called before the first frame update
    void Start()
    {
        planets = GameObject.FindGameObjectsWithTag("Planet"); //Try to dynamically record all planets
        rigidBodies = FindObjectsOfType<Rigidbody2D>(); //Try to dynamically record all rigid bodies
    }

    //Logic to update rigid body
    void Update()
    {
        //Update Arrays:
        planets = GameObject.FindGameObjectsWithTag("Planet"); //Try to dynamically record all planets
        rigidBodies = FindObjectsOfType<Rigidbody2D>(); //Try to dynamically record all rigid bodies
        foreach (Rigidbody2D currentRB in rigidBodies)
        {
            Transform transform = currentRB.transform; //Refers to transform of current RB
            float gravX = 0;
            float gravY = 0;
            float closestRadius = 1000000000;
            for (int i = 0; i < planets.Length; i++)
            {
                planetMass = planets[i].GetComponent<PlanetStats>().mass; //Sets planet mass to stored value
                gravityx = -1 * (transform.position.x - planets[i].transform.position.x);
                gravityy = -1 * (transform.position.y - planets[i].transform.position.y);
                c = Mathf.Sqrt((gravityx * gravityx) + (gravityy * gravityy));
                if (c < closestRadius)
                {
                    closestRadius = c;
                }
                gravX += gravityx * gravitationalConstant * currentMass * planetMass / c / c / c;
                gravY += gravityy * gravitationalConstant * currentMass * planetMass / c / c / c; //Accumulates grav X and Y based upon list of planets, relative to the current transform
            }
            float currentSpeed = Mathf.Sqrt((currentRB.velocity.x * currentRB.velocity.x) + (currentRB.velocity.y * currentRB.velocity.y));
            if (currentSpeed < 5f && orbitalAssist && closestRadius > 7 && closestRadius < 8) //Checks for orbit conditions, maybe make the radius value be dependent on planet
            {
                Debug.Log("Assisting");
                currentRB.velocity = new Vector2(-gravY/100, gravX/100); //Sets velocity perpendicular to net gravity (maybe make this just for closest planet gravity)
                //Doesn't apply gravity if moving in parallel - in order to apply orbital assist
                //currentRB.AddForce(new Vector2(-gravY, gravX)); //Orbital assist when perpendicular to gravity (parallel)
                //currentRB.AddForce(new Vector2(gravX, gravY)); //Modifies gravity based upon planet locations - try to modify this global version if it doesnt work: Physics2D.gravity = new Vector2(gravX, gravY);

            }
            else
            {
                currentRB.AddForce(new Vector2(gravX, gravY)); //Modifies gravity based upon planet locations - try to modify this global version if it doesnt work: Physics2D.gravity = new Vector2(gravX, gravY);

            }
        }
    }
}
