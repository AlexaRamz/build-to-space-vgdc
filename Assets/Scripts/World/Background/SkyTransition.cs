using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyTransition : MonoBehaviour
{
    Color32 spaceColor = new Color32(57, 0, 86, 255);
    public float minHeight = 0f;
    public float spaceHeight = 800f;
    public float teleportHeight = 1000f;
    [SerializeField] SpriteRenderer sky;

    private void Update()
    {
        float currentHeight = transform.position.y;

        // Change sky color based on height
        float value = (currentHeight - minHeight) / (spaceHeight - minHeight);
        Color currentColor = Color.Lerp(new Color32(255, 255, 255, 255), spaceColor, value);

        sky.color = currentColor;

        // Fade in space overlay based on height
        if (currentHeight >= spaceHeight)
        {
            value = (currentHeight - spaceHeight) / (teleportHeight - spaceHeight);
            TransitionManager.Instance.FadeOverlay(value, spaceColor);
        }
        if (currentHeight >= teleportHeight)
        {
            TransitionManager.Instance.SaveCurrentShip();
            SceneLoader.Instance.LoadScene("SpaceScene");
        }
    }
}
