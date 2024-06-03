using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlanetStats : MonoBehaviour
{
    [SerializeField]
    public float mass = 2; //Sets default mass value to 2 for a planet
    [SerializeField]
    public float minimumOrbit = 0f;
    [SerializeField]
    public float maximumOrbit = 0f;
}
