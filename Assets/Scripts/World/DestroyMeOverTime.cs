using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DestroyMeOverTime : MonoBehaviour
{

    public GameObject CreateOnDestroy;
    public float DelayTime;
    public bool RequireStartCreationFinished;

    void Start()
    {
        StartCoroutine(DelayedDestroy());
    }

    public IEnumerator DelayedDestroy()
    {

        if (RequireStartCreationFinished)
        {
            while (GetComponent<EnemyAttack>() != null && GetComponent<EnemyAttack>().CreateOnStart != null && !GetComponent<EnemyAttack>().hasCreatedStart)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        yield return new WaitForSeconds(DelayTime);
        
        if(CreateOnDestroy!= null)
        {
            Instantiate(CreateOnDestroy,transform.position,transform.rotation,transform.parent);
        }
        Destroy(gameObject);
    }
}
