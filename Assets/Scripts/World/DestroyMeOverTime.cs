using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DestroyMeOverTime : MonoBehaviour
{

    public GameObject CreateOnDestroy;
    public float DelayTime;

     void Start()
    {
        StartCoroutine(DelayedDestroy());
    }

    public IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(DelayTime);
        if(CreateOnDestroy!= null)
        {
            Instantiate(CreateOnDestroy,transform.position,transform.rotation,transform.parent);
        }
        Destroy(gameObject);
    }
}
