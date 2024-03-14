using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerInteraction : MonoBehaviour
{
    // Handles interaction by trigger and mouse click
    public OnTriggerDo interactTrigger;
    bool interactablesInRange = false;
    Interactable currentInteract;
    MenuManager menuManager;
    public bool canInteract = true;
    public GameObject arrowPrefab;
    GameObject arrow;
    const float updateRate = 6; // updates per second
    bool pointing;

    private void Awake()
    {
        menuManager = GameObject.Find("MenuManager").GetComponent<MenuManager>();
        arrow = Instantiate(arrowPrefab);
    }
    private void Start()
    {
        StartCoroutine(Repeat_UpdateInteract());
    }

    IEnumerator Repeat_UpdateInteract()
    {
        while (true)
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
        GameObject pointerOn = GetPointerOn();
        if (pointerOn != null)
        {
            Interactable newInteract = pointerOn.GetComponent<Interactable>();
            if (newInteract && !newInteract.beingUsed)
            {
                pointing = true;
                return newInteract;
            }
        }
        pointing = false; 

        //RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, new Vector2(1.5f, 1.5f), 0, Vector2.zero);
        List<GameObject> objectsInRange = interactTrigger.GetObjectsInRange();
        Interactable closestObject = null;
        float closestDistance = 100f;
        //foreach (RaycastHit2D rc in hits)
        foreach (GameObject obj in objectsInRange)
        {
            //GameObject obj = rc.transform.gameObject;
            Interactable I = obj.GetComponent<Interactable>();
            if (I != null && !I.beingUsed)
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
    public float arrowOffset = 0.25f;
    void InteractIconOn(Interactable thisInteract)
    {
        arrow.transform.Find("Image").GetComponent<SpriteRenderer>().enabled = true;
        arrow.GetComponent<Animator>().SetBool("InRange", true);
        Transform thisObject = thisInteract.transform;
        arrow.transform.position = thisObject.position + new Vector3(0, thisObject.GetComponent<SpriteRenderer>().bounds.size.y / 2 + arrowOffset, 0);
    }
    void InteractIconOff(Interactable thisInteract)
    {
        arrow.transform.Find("Image").GetComponent<SpriteRenderer>().enabled = false;
        arrow.GetComponent<Animator>().SetBool("InRange", false);
    }

    public GameObject GetPointerOn()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorldPos, Vector2.zero);
        List<GameObject> Objects = new List<GameObject>();
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null && hits[i].collider.gameObject.GetComponent<Interactable>())
            {
                Objects.Add(hits[i].collider.gameObject);
            }
        }
        Objects = Objects.OrderBy(e => e.transform.position.y).ToList();
        GameObject hit = null;
        if (Objects.Count > 0)
        {
            hit = Objects[0];
        }
        return hit;
    }

    void Update()
    {
        if (canInteract)
        {
            if ((Input.GetKeyDown(KeyCode.Return)) && currentInteract != null)
            {
                currentInteract.Interact();
            }
            if (pointing && Input.GetMouseButtonDown(0) && currentInteract != null)
            {
                currentInteract.Interact();
            }
        }
        canInteract = !menuManager.IsInMenu();
    }
}
