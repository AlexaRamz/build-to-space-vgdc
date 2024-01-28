using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    // Handles interaction by trigger and mouse click
    public OnTriggerDo interactTrigger;
    bool interactablesInRange = false;
    Interactable currentInteract;
    MenuManager menuManager;

    private void Start()
    {
        menuManager = GameObject.Find("MenuManager").GetComponent<MenuManager>();
    }
    IEnumerator currentCoroutine;
    public void UpdateCurrentInteract() // Called on trigger enter
    {
        UpdateInteract();
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = Repeat_UpdateCurrentInteract();
        StartCoroutine(currentCoroutine); // Update as long as there are interactables in range
    }
    const float updateRate = 10; // updates per second
    IEnumerator Repeat_UpdateCurrentInteract()
    {
        while (interactablesInRange)
        {
            UpdateInteract();
            yield return new WaitForSeconds(1 / updateRate);
        }
    }
    void UpdateInteract()
    {
        Interactable newInteract = GetInteractingWith();
        interactablesInRange = newInteract != null;
        if (currentInteract != newInteract)
        {
            if (currentInteract != null) InteractIconOff(currentInteract);
            if (newInteract != null) InteractIconOn(newInteract);
            currentInteract = newInteract;
        }
    }
    Interactable GetInteractingWith()
    {
        //RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, new Vector2(1.5f, 1.5f), 0, Vector2.zero);
        List<GameObject> objectsInRange = interactTrigger.GetObjectsInRange();
        Interactable closestObject = null;
        float closestDistance = 100f;
        //foreach (RaycastHit2D rc in hits)
        foreach (GameObject obj in objectsInRange)
        {
            //GameObject obj = rc.transform.gameObject;
            if (obj.GetComponent<Interactable>())
            {
                float distance = Vector2.Distance(obj.transform.position, transform.position);
                if (distance < closestDistance)
                {
                    closestObject = obj.GetComponent<Interactable>();
                    closestDistance = distance;
                }
            }
        }
        return closestObject;
    }
    public GameObject arrow;
    public float arrowOffset = 0.25f;
    void InteractIconOn(Interactable thisInteract)
    {
        arrow.transform.Find("Image").GetComponent<SpriteRenderer>().enabled = true;
        arrow.GetComponent<Animator>().SetBool("InRange", true);
        Transform thisObject = thisInteract.transform;
        arrow.transform.position = thisObject.position + new Vector3(0, thisObject.GetComponent<SpriteRenderer>().bounds.size.y / 2 + arrowOffset, 0);
        arrow.transform.SetParent(thisObject);
    }
    void InteractIconOff(Interactable thisInteract)
    {
        arrow.transform.Find("Image").GetComponent<SpriteRenderer>().enabled = false;
        arrow.GetComponent<Animator>().SetBool("InRange", false);
        arrow.transform.SetParent(transform);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !menuManager.IsInMenu() && currentInteract != null)
        {
            currentInteract.Interact();
        }
    }
}
