using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class QuestFinder : MonoBehaviour
{
    // Purpose is to create an arrow on screen/the edge of the screen if the target is off screen, which points to the current quest objective
    
    public float positionOffset = 25f;
    public float fadeTime = 0.475f; //Time in order to fade color in or out
    bool colorStatus = false; //False used for clear, true used for white (default is clear)
    Color startColor = new Color(1f, 1f, 1f, 0f); //Default color set to clear
    Color endColor = Color.white; //Transition in color set to white
    public Transform questTarget;
    public RectTransform questMarker;

    UnityEngine.UI.Image questMarkerImage; //Stores image from questMarker in order to modify its' alpha characteristic

    // Start is called before the first frame update
    void Start()
    {
        if (questMarker != null)
        {
            questMarkerImage = questMarker.gameObject.GetComponent<UnityEngine.UI.Image>(); //Sets the image to the image associated with the quest marker object
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (questTarget != null && questMarker != null)
        {
            UnityEngine.Vector2 destinationPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, questTarget.position); //Determines location of destination

            UnityEngine.Vector3 screenPoint = Camera.main.WorldToScreenPoint(questTarget.position);
            if (screenPoint.x < 0 || screenPoint.x > Screen.width || screenPoint.y < 0 || screenPoint.y > Screen.height) //Destination is off screen
            {
                if (!colorStatus)
                {
                    StartCoroutine(FadeImage(true)); //Enables marker when target is off screen
                    colorStatus = true;
                }
                
                UnityEngine.Vector2 canvasLocation = Camera.main.ScreenToViewportPoint(destinationPosition); //Determines location relative to canvas, constrained between 0 and 1

                //Recording Relative Position:
                UnityEngine.Vector2 edgeLocation = destinationPosition; //Variable used to determine edge location, initially set to destination prior to shifting
                if (canvasLocation.x < 0)
                {
                    edgeLocation.x = positionOffset; //Limits lower bound of x to 0
                }
                else if (canvasLocation.x > 1)
                {
                    edgeLocation.x = Screen.width-positionOffset; //Sets upper bound of x to the screen width
                }
                if (canvasLocation.y < 0)
                {
                    edgeLocation.y = positionOffset; //Limits lower bound of y to 0
                }
                else if (canvasLocation.y > 1)
                {
                    edgeLocation.y = Screen.height-positionOffset; //Sets upper bound of y to the screen height
                }
                //Setting Relative Position:
                questMarker.position = edgeLocation;
                
                //Recording Relative Rotation: - Need to test this on a level with more verticality
                UnityEngine.Vector2  relativeDirection = destinationPosition-edgeLocation; //Determines the direction from where the marker is placed, to the destination object
                float relativeAngle = Mathf.Atan2(relativeDirection.y, relativeDirection.x) * Mathf.Rad2Deg; //Determines angle between marker and destination
                questMarker.rotation = UnityEngine.Quaternion.Euler(0, 0, relativeAngle); //Sets angle based upon the given z aspect
            }
            else //Destination is on screen
            {
                if (colorStatus)
                {
                    StartCoroutine(FadeImage(false)); //Disables marker when target is on screen
                    colorStatus = false;
                }
            }
        }
    }

    IEnumerator FadeImage(bool fadeMode) //Function to change color to endColor - fadeMode is true for start->end i.e. clear->white, and false for the other direction
    {
        //Determines the start and end color for this particular instance of the function:
        Color Start;
        Color End;
        if (fadeMode)
        {
            Start = startColor;
            End = endColor;
        }
        else
        {
            Start = endColor;
            End = startColor;
        }
        
        float activeTime = 0f;

        while (activeTime<fadeTime) //determines whether or not it is still fading
        {
            float timeFactor = activeTime/fadeTime;
            questMarkerImage.color = Color.Lerp(Start, End, timeFactor);
            activeTime += Time.deltaTime;
            yield return null; //Waits for next frame
        }

        questMarkerImage.color = End; //Sets to ending color when 
    }

    public void UpdateTarget(Transform newLocation)
    {
        questTarget = newLocation; 
    }
}
