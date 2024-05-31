using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlanetTransition : MonoBehaviour
{
    public string goToScene;
    public Color32 atmosphereColor;
    float transitionStartDistance;
    public float transitionBufferSize = 5f;
    float teleportDistance;
    GameObject plr;
    bool entered = false;

    private void Start()
    {
        plr = GameObject.FindGameObjectWithTag("Player");
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        transitionStartDistance = Mathf.Min(sr.bounds.size.x, sr.bounds.size.y) / 2.0f;
        teleportDistance = transitionStartDistance - transitionBufferSize;
    }

    bool canReset;
    private void Update()
    {
        if (entered) return;
        float plrDistance = Vector2.Distance(plr.transform.position, transform.position);

        float value = (transitionStartDistance - plrDistance) / (transitionStartDistance - teleportDistance);
        if (plrDistance <= transitionStartDistance)
        {
            TransitionManager.Instance.FadeOverlay(value, atmosphereColor);
            canReset = true;
        }
        else
        {
            if (canReset)
            {
                TransitionManager.Instance.ResetOverlay();
                canReset = false;
            }
        }

        if (plrDistance <= teleportDistance)
        {
            TransitionManager.Instance.SaveCurrentShip();
            SceneManager.LoadScene(goToScene);
            entered = true;
        }
    }
}
