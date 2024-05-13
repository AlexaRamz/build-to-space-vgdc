using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeightTracker : MonoBehaviour
{
    float initialHeight;
    float maximumHeight;
    TextMeshProUGUI highscoreDisplayer;

    public Transform playerLocation; //Reference to player's position
    public RectTransform heightTracker; //Reference to UI element that tracks highest point reached
    int Visible = 1; //Check for visibility - starts as on, (H) to toggle

    // Start is called before the first frame update
    void Start()
    {
        initialHeight = playerLocation.position.y; //Records initial height point
        maximumHeight = 0f; //Sets initial maximum to 0
        if (heightTracker != null)
        {
            highscoreDisplayer = heightTracker.GetComponent<TextMeshProUGUI>();
            highscoreDisplayer.text = "Highscore: 0.000000"; //Sets initial value to 0 before anything has properly registered
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) //Swaps visibility before determining how to set text
            SwapVisibility();
        if (Visible == 1)
        {
            float currentPosition = playerLocation.position.y;
            if ((currentPosition - initialHeight) > maximumHeight)
            {
                maximumHeight = currentPosition - initialHeight;
                UpdateText(maximumHeight);
            }
        }
        else
        {
            highscoreDisplayer.text = ""; //Sets text to blank
        }
    
    }

    void SwapVisibility()
    {
        if (Visible == 1)
        {
            Visible = 0;
        }
        else
        {
            Visible = 1;
            UpdateText(maximumHeight);
        }
    }

    void UpdateText(float newScore)
    {
        if (newScore > 0.01) //Sets minimum threshold so value is initially 0
        {
            if (highscoreDisplayer != null) //Only utilizes the set parameters for text elements
            {
                highscoreDisplayer.text = "Highscore: " + newScore;
            }
        }
        else
        {
            highscoreDisplayer.text = "Highscore: 0.000000"; //Needed to properly display when turning on Highscore UI without having moved first
        }
        /*if (sphereTracker != null) //Implementation of breakpoints could be adjusted based upon how fast spaceships are
        {
            if (newScore > 1000)
            {
                sphereTrackerDisplayer.text = "Stratosphere";
            }
            else if (newScore > 3000)
            {
                sphereTrackerDisplayer.text = "Mesosphere";
            }
            else if (newScore > 5000)
            {
                sphereTrackerDisplayer.text = "Thermosphere";
            }
            else if (newScore > 40000)
            {
                sphereTrackerDisplayer.text = "Exosphere";
            }
        }*/ //Sphere logic
    }

}
