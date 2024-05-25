using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{
    private float gravityx;
    private float gravityy; //Records net gravity values
    private float c; //Used in gravity calculation
    private float currentMass = 1; //Set to 1 as a default, used to record current rigid body mass
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
            for (int i = 0; i < planets.Length; i++)
            {
                planetMass = planets[i].GetComponent<PlanetStats>().mass; //Sets planet mass to stored value
                gravityx = -1 * (transform.position.x - planets[i].transform.position.x);
                gravityy = -1 * (transform.position.y - planets[i].transform.position.y);
                c = Mathf.Sqrt(gravityx * gravityx) + (gravityy * gravityy);
                gravX += gravityx * gravitationalConstant * currentMass * planetMass / c / c / c;
                gravY += gravityy * gravitationalConstant * currentMass * planetMass / c / c / c; //Accumulates grav X and Y based upon list of planets, relative to the current transform
            }
            currentRB.AddForce(new Vector2(gravX, gravY)); //Modifies gravity based upon planet locations - try to modify this global version if it doesnt work: Physics2D.gravity = new Vector2(gravX, gravY);
        }
    }
}
