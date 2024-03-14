using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : Interactable
{
    PlayerMovement plrMove;
    public Transform seatPoint;

    void Start()
    {
        plrMove = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }
    public override void Interact()
    {
        if (seatPoint != null) plrMove.Sit(this);
    }
}
