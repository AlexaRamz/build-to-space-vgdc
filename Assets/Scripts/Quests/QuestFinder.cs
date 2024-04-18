using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class QuestFinder : MonoBehaviour
{
    //The purpose of this file is to create an arrow on screen/the edge of the screen if the target is off screen, which points to the current quest objective
    
    MenuManager menuManager;
    Canvas canvas;

    public Transform questTarget;
    public RectTransform questMarker;

    // Start is called before the first frame update
    void Start()
    {
        menuManager = MenuManager.Instance;
        canvas = menuManager.GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        if (questTarget != null && canvas != null && questMarker != null)
        {
            UnityEngine.Vector2 destinationPosition = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, questTarget.position); //Determines location of destination

            if (!RectTransformUtility.RectangleContainsScreenPoint(canvas.GetComponent<RectTransform>(), destinationPosition)) //Destination is off screen
            {
                UnityEngine.Vector2 canvasLocation = Camera.main.ScreenToViewportPoint(destinationPosition); //Determines location relative to canvas, constrained between 0 and 1

                //Recording Relative Position:
                UnityEngine.Vector2 edgeLocation = destinationPosition; //Variable used to determine edge location, initially set to destination prior to shifting
                if (canvasLocation.x < 0)
                {
                    edgeLocation.x = 0; //Limits lower bound of x to 0
                }
                else if (canvasLocation.x > 1)
                {
                    edgeLocation.x = Screen.width; //Sets upper bound of x to the screen width
                }
                if (canvasLocation.y < 0)
                {
                    edgeLocation.y = 0; //Limits lower bound of y to 0
                }
                else if (canvasLocation.y > 1)
                {
                    edgeLocation.y = Screen.height; //Sets upper bound of y to the screen height
                }
                //Setting Relative Position:
                questMarker.position = edgeLocation;
                
                //Recording Relative Rotation:
            }
            else //Destination is on screen
            {
                questMarker.gameObject.SetActive(false); //Disables marker when target is on screen
            }
        }
    }
}
