using UnityEngine;
using UnityEngine.Tilemaps;

public class Worldgen : MonoBehaviour
{
    [SerializeField]
    private WorldTiles Tiles;
    [SerializeField]
    private Tilemap Foreground;
    [SerializeField]
    private Tilemap Background;
    private void Start()
    {
        GenerateWorld();
        CenterInPlaySpace();
        try //There is a bug happening here in the build but not the dev version..?
        {
            TerrainManager.Instance?.AddGroundTiles(); //There is already a reference and call to this in BuildingSystem.cs, but it needs to run after the world is generated. It probably should be moved appropriately.
        }
        catch
        {

        }
    }
    private void Update()
    {
        
    }
    private const int WorldWidth = 400;
    private const int WorldHeight = 100;
    private void GenerateWorld()
    {
        float randSeed = Random.Range(0, 256f);
        float smoothness = 40f;
        float caveSmoothnessX = 24f;
        float caveSmoothnessY = 20f;
        float minHeightPercent = 0.8f;
        float caveSize = 0.075f;
        float caveFrequency = 0.42f;
        //This generates the shape of the world, including background tiles and caves.
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
                    Vector3Int pos = new Vector3Int(i, j);
                    if (notACave)
                    {
                        Foreground.SetTile(pos, Tiles.Cracked);
                    }
                    Background.SetTile(pos, Tiles.Cracked);
                    Background.SetColor(pos, Color.gray);
                }
            }
        }
        //This generates the dead bushes on the surface of the world, only in the background.
        for (int i = 0; i < WorldWidth; i++)
        {
            for (int j = WorldHeight; j > 0; j--)
            {
                Vector3Int belowRPos = new Vector3Int(i + 1, j - 1);
                Vector3Int belowLPos = new Vector3Int(i - 1, j - 1);
                Vector3Int belowPos = new Vector3Int(i, j - 1);
                Vector3Int pos = new Vector3Int(i, j);
                if (Foreground.HasTile(belowPos) && !Background.HasTile(belowPos))
                {
                    if (Foreground.HasTile(belowLPos) && Foreground.HasTile(belowRPos) && Random.Range(0, 1f) < 0.1f)
                        Background.SetTile(pos, Tiles.DeadBush);
                    break;
                }
            }
        }
        //Generate ores... since there are no ores for now, I'm generating purple tiles.
        for (int i = 0; i < WorldWidth; i++)
        {
            for (int j = 0; j < WorldHeight; j++)
            {
                Vector3Int pos = new Vector3Int(i, j);
                if (Foreground.HasTile(pos))
                {
                    if (Random.Range(0, 1f) < 0.005f)
                    {
                        GenerateOre(i, j, Random.Range(100, 200));
                    }
                }
            }
        }
    }
    private void GenerateOre(int i, int j, int size, float centeringBias = 0.03f)
    {
        Vector3 center = new Vector3(i, j);
        Vector3 pos = new Vector3(i, j);
        while (size > 0)
        {
            Vector3Int IntPos = new Vector3Int((int)pos.x, (int)pos.y);
            if (Foreground.HasTile(IntPos))
            {
                Foreground.SetTile(IntPos, Tiles.Purple);
            }
            pos.x += Random.Range(-.5f, .5f);
            pos.y += Random.Range(-.5f, .5f);
            pos = Vector3.Lerp(pos, center, centeringBias);
            size--;
        }
    }
    private void CenterInPlaySpace()
    {
        int offsetY = WorldHeight;
        int offsetX = WorldWidth / 2;
        transform.position = new Vector3(transform.position.x - offsetX / 2, transform.position.y - offsetY);
    }
}
