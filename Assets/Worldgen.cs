using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Worldgen : MonoBehaviour
{
    [SerializeField]
    private Tile DeadBush;
    [SerializeField]
    private RuleTile Cracked;
    [SerializeField]
    private Tilemap Foreground;
    [SerializeField]
    private Tilemap Background;
    private void Start()
    {
        GenerateWorld();
    }
    private void Update()
    {
        
    }
    private int WorldWidth = 400;
    private int WorldHeight = 100;
    private void GenerateWorld()
    {
        float randSeed = Random.Range(0, 256f);
        float smoothness = 40f;
        float caveSmoothnessX = 24f;
        float caveSmoothnessY = 20f;
        float minHeightPercent = 0.8f;
        int offsetY = WorldHeight;
        int offsetX = WorldWidth / 2;
        float caveSize = 0.075f;
        float caveFrequency = 0.42f;
        for (int i = 0; i < WorldWidth; i++)
        {
            float elevationNoise = minHeightPercent + (1 - minHeightPercent) * Mathf.PerlinNoise1D((float)(i + randSeed) / smoothness);
            for (int j = 0; j < WorldHeight; j++)
            {
                float heightPercent = ((float)j / WorldHeight);
                float indentNoise = Mathf.PerlinNoise(i / caveSmoothnessX, j / caveSmoothnessY);
                float diffElevation = Mathf.Clamp(heightPercent - elevationNoise, -1, 1);
                bool notACave = (indentNoise > caveFrequency + caveSize || indentNoise < caveFrequency - caveSize);
                if (diffElevation <= 0)
                {
                    Vector3Int pos = new Vector3Int(i - offsetX, j - offsetY);
                    if (notACave)
                    {
                        Foreground.SetTile(pos, Cracked);
                    }
                    Background.SetTile(pos, Cracked);
                    Background.SetColor(pos, Color.gray);
                }
            }
        }
        for(int i = 0; i < WorldWidth; i++)
        {
            for(int j = WorldHeight; j > 0; j--)
            {
                Vector3Int belowRPos = new Vector3Int(i - offsetX + 1, j - offsetY - 1);
                Vector3Int belowLPos = new Vector3Int(i - offsetX - 1, j - offsetY - 1);
                Vector3Int belowPos = new Vector3Int(i - offsetX, j - offsetY - 1);
                Vector3Int pos = new Vector3Int(i - offsetX, j - offsetY);
                if (Foreground.HasTile(belowPos))
                {
                    if(Foreground.HasTile(belowLPos) && Foreground.HasTile(belowRPos) && Random.Range(0, 1f) < 0.1f)
                        Background.SetTile(pos, DeadBush);
                    break;
                }
            }
        }
    }
}
