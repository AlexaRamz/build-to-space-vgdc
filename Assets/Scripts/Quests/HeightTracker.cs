using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeightTracker : MonoBehaviour
{
    float initialHeight;
    float maximumHeight;
    TextMeshProUGUI highscoreDisplayer;
    TextMeshPro stationaryHighscoreDisplayer;
    TextMeshPro sphereTrackerDisplayer;

    public Transform playerLocation; //Reference to player's position
    public RectTransform heightTracker; //Reference to UI element that tracks highest point reached
    public RectTransform stationaryHeightTracker; //Reference to Stationary Text element that tracks highest point reached - The expectation is that this will be set to a specific element for a given level
                                                    //An example of this usage would be for a notice board style of display (see QuestTest level for an example)
    public RectTransform sphereTracker;

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
        if (stationaryHeightTracker != null)
        {
            stationaryHighscoreDisplayer = stationaryHeightTracker.GetComponent<TextMeshPro>();
            stationaryHighscoreDisplayer.text = "Highscore: 0.000000"; //Sets initial value to 0 before anything has properly registered
        }
        if (sphereTracker != null)
        {
            sphereTrackerDisplayer = sphereTracker.GetComponent<TextMeshPro>();
            sphereTrackerDisplayer.text = "Troposphere"; //Sets initial text
        }
    }

    // Update is called once per frame
    void Update()
    {
        float currentPosition = playerLocation.position.y;
        if ((currentPosition - initialHeight) > maximumHeight)
        {
            maximumHeight = currentPosition - initialHeight;
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
            if (stationaryHighscoreDisplayer != null)
            {
                stationaryHighscoreDisplayer.text = "Highscore: " + newScore;
            }
        }
        if (sphereTracker != null) //Implementation of breakpoints could be adjusted based upon how fast spaceships are
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
        }
    }

}
