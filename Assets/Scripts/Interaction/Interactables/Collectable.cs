using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public Item itemInfo;
    bool canCollect = true;
    bool Debounce = false;
    public void SetItem(Item info)
    {
        GetComponent<SpriteRenderer>().sprite = info.image;
        itemInfo = info;
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
            col.gameObject.GetComponent<PlayerManager>().AddToInventory(itemInfo, 1);
            Destroy(gameObject);
        }
    }
}
