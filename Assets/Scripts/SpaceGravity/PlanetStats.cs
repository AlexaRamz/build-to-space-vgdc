using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetStats : MonoBehaviour
{
    [SerializeField]
    public float mass = 2; //Sets default mass value to 2 for a planet
    [SerializeField]
    public SceneAsset scene;
    [SerializeField]
    public float minimumOrbit = 0f;
    [SerializeField]
    public float maximumOrbit = 0f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (scene != null)
        {
            string sceneString = System.IO.Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(scene));
            SceneManager.LoadScene(sceneString); //Loads level when collided with
        }
    }
}
