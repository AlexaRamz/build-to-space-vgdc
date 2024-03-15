using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUp : MonoBehaviour
{
    public void ClosePopUp()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
