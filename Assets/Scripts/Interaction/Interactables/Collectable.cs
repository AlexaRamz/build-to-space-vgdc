using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public ResourceType resource;
    bool canCollect = true;
    bool Debounce = false;
    public void SetItem(Resource info)
    {
        GetComponent<SpriteRenderer>().sprite = info.image;
        resource = info.type;
        canCollect = false;
        StartCoroutine(CollectDelay());
    }
    IEnumerator CollectDelay()
    {
        yield return new WaitForSeconds(0.2f);
        canCollect = true;
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (canCollect && col.gameObject.CompareTag("Player") && !Debounce)
        {
            Debounce = true;
            col.gameObject.GetComponent<Inventory>().Collect(resource);
            Destroy(gameObject);
        }
    }
}
