using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour
{
    public virtual ToolData data { get; set; }

    private bool readyToUse = true;

    IEnumerator useDelay()
    {
        yield return new WaitForSeconds(data.activationCooldown);
        readyToUse = true;
    }

    public virtual bool Use()
    {
        if (!readyToUse) return false;

        readyToUse = false;
        StartCoroutine(useDelay());
        return true;
    }
}
