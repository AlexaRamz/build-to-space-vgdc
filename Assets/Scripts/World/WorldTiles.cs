using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
[CreateAssetMenu(fileName = "New WorldTileCatalog", menuName = "Scriptable Objects/World Tile Catalog")]
public class WorldTiles : ScriptableObject
{
    public Tile DeadBush;
    public RuleTile Dirt;
    public RuleTile Purple;
    public RuleTile Stone;
    public RuleTile CopperOre;
    public RuleTile IronOre;
    public RuleTile AluminumOre;
    public RuleTile CoalOre;
}
