using UnityEngine;
using UnityEngine.Rendering;
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
    private const int WorldHeight = 150;
    private void GenerateWorld()
    {
        float stoneLayer = 0.85f; //How high up does stone start spawning?

        float randSeed = Random.Range(0, 256f);
        float smoothness = 80f;
        float caveSmoothnessX = 24f;
        float caveSmoothnessY = 20f;
        float minHeightPercent = 0.9f;
        float caveFrequency = 0.42f;
        //This generates the shape of the world, including background tiles and caves.
        for (int j = 0; j < WorldHeight; j++)
        {
            float caveSize = 0.075f;
            float heightPercent = (float)j / WorldHeight;
            if (heightPercent < 0.6f)
            {
                float thinCaves = Mathf.Sqrt(heightPercent / 0.6f);
                caveSize *= thinCaves;
            }
            for (int i = 0; i < WorldWidth; i++)
            {
                float elevationNoise = minHeightPercent + (1 - minHeightPercent) * Mathf.PerlinNoise1D((float)(i + randSeed) / smoothness);
                float indentNoise = Mathf.PerlinNoise(i / caveSmoothnessX, j / caveSmoothnessY);
                float diffElevation = Mathf.Clamp(heightPercent - elevationNoise, -1, 1);
                bool notACave = (indentNoise > caveFrequency + caveSize || indentNoise < caveFrequency - caveSize);
                if (diffElevation <= 0)
                {
                    Vector3Int pos = new Vector3Int(i, j);
                    var TileType = Tiles.Dirt;
                    float stoneLayerRandomness = Mathf.Abs(Mathf.PerlinNoise(i / 25f, j / 15f));
                    if(heightPercent < stoneLayer + 0.05f * stoneLayerRandomness)
                    {
                        TileType = Tiles.Stone;
                    }
                    if (notACave)
                    {
                        Foreground.SetTile(pos, TileType);
                    }
                    Background.SetTile(pos, TileType);
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
                if (Foreground.HasTile(belowPos) && !Background.HasTile(pos))
                {
                    if (Foreground.HasTile(belowLPos) && Foreground.HasTile(belowRPos) && Random.Range(0, 1f) < 0.1f)
                        Background.SetTile(pos, Tiles.DeadBush);
                    break;
                }
            }
        }


        PlaceOres(80, new Vector2Int(100, 150), 0.85f, Tiles.CoalOre);
        PlaceOres(60, new Vector2Int(100, 200), 0.75f, Tiles.CopperOre);
        PlaceOres(40, new Vector2Int(100, 200), 0.6f, Tiles.IronOre, 0.02f);
        PlaceOres(20, new Vector2Int(100, 250), 0.4f, Tiles.AluminumOre, -0.03f); //Having a negative centering bias makes it generate in less clumpy, more worm-like shapes!
    }
    private void PlaceOres(float totalVeins, Vector2Int veinSize, float maximumHeightPercent, TileBase type, float centeringBias = 0.03f)
    {
        while(totalVeins > 0)
        {
            Vector3Int pos = new Vector3Int((int)(WorldWidth * Random.Range(0, 1f)), (int)(WorldHeight * Random.Range(0, maximumHeightPercent)));
            if (Foreground.HasTile(pos))
            {
                GenerateOre(pos.x, pos.y, Random.Range(veinSize.x, veinSize.y), type, centeringBias);
                totalVeins--;
            }
            else
                totalVeins -= 0.1f; //If we fail to generate an ore, make it count as using 1/10th of an ore slot
        }
    }
    private void GenerateOre(int i, int j, int size, TileBase oreType, float centeringBias = 0.03f)
    {
        Vector3 center = new Vector3(i, j);
        Vector3 pos = new Vector3(i, j);
        while (size > 0)
        {
            Vector3Int IntPos = new Vector3Int((int)pos.x, (int)pos.y);
            if (Foreground.HasTile(IntPos))
            {
                Foreground.SetTile(IntPos, oreType);
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
