using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : Interactable
{
    public Transform seatPoint;

    public override void Interact()
    {
        PlayerMovement plrMove = GameObject.Find("Player").GetComponent<PlayerMovement>();
        if (seatPoint != null) plrMove.Sit(this);
    }
}
