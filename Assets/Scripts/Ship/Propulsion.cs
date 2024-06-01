using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propulsion : Interactable
{
    public bool stayOn;
    public bool isOn;
    public float forceMagnitude = 3f;
    [SerializeField] ParticleSystem thrustParticles;
    [SerializeField] Transform forcePoint;
    public float forceAngle;
    public Ship ship;

    public override void Interact()
    {
        ToggleThrust();
        stayOn = isOn;
    }

    public void ToggleThrust()
    {
        isOn = !isOn;
        if (isOn)
        {
            thrustParticles.Play();
        }
        else
        {
            thrustParticles.Stop();
        }
    }
    public void ThrustOn()
    {
        if (!isOn)
            ToggleThrust();
    }
    public void ThrustOff()
    {
        if (!stayOn && isOn)
        {
            ToggleThrust();
        }
    }

    public void FixedUpdate()
    {
        if (isOn && ship != null)
        {
            ship.ApplyForce(forcePoint.position, transform.eulerAngles.z, forceMagnitude);
        } 
    }
}
