using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool beingUsed;
    public abstract void Interact();
    public bool showIcon = true;
    public bool interactOnEnter = true;
}
