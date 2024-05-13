using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreSource : MonoBehaviour
{
	public ResourceDeposit resouceDepositInfo;
	
    public void SetItem(ResourceDeposit info)
    {
        GetComponent<SpriteRenderer>().sprite = info.image;
        resouceDepositInfo = info;
    }
}