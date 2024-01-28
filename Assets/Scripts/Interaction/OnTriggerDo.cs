using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerDo : MonoBehaviour
{
    public UnityEvent actionOnEnter;
    public UnityEvent actionOnExit;
    public List<string> tags;

    List<GameObject> objectsInRange = new List<GameObject>();
    public List<GameObject> GetObjectsInRange()
    {
        return objectsInRange;
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (tags.Count == 0)
        {
            actionOnEnter.Invoke();
            objectsInRange.Add(col.gameObject);
        }
        foreach (string thisTag in tags)
        {
            if (col.gameObject.tag == thisTag)
            {
                objectsInRange.Add(col.gameObject);
                actionOnEnter.Invoke();
                return;
            }
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (tags.Count == 0)
        {
            actionOnExit.Invoke();
            objectsInRange.Remove(col.gameObject);
        }
        foreach (string thisTag in tags)
        {
            if (col.gameObject.tag == thisTag)
            {
                bool objectMatches(GameObject obj)
                {
                    return obj == col.gameObject;
                }
                objectsInRange.RemoveAll(objectMatches);
                actionOnExit.Invoke();
                return;
            }
        }
    }
}
