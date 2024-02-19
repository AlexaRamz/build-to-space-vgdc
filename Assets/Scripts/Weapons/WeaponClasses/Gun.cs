using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour, ITool
{
    private bool readyToUse = false;
    private GunData gunData;
    public ToolData data
    {
        get
        {
            return gunData;
        }
        set
        {
            if (value.GetType() != typeof(GunData))
            {
                Debug.Log("Incorrect data type!");
                return;
            }
            gunData = (GunData)value;
            GetComponent<SpriteRenderer>().sprite = gunData.sprite;
            readyToUse = true;
        }
    }

    IEnumerator useDelay()
    {
        yield return new WaitForSeconds(gunData.fireCoolDown);
        readyToUse = true;
    }

    public void Use()
    {
        if (readyToUse)
        {
            readyToUse = false;
            StartCoroutine(useDelay());
            Shoot();
        }
    }

    public void Shoot()
    {
        Instantiate(gunData.bulletPrefab, transform.position, Quaternion.identity);
    }
}
