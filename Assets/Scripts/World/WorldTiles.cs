using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
[CreateAssetMenu(fileName = "New WorldTileCatalog", menuName = "Scriptable Objects/World Tile Catalog")]
public class WorldTiles : ScriptableObject
{
    public Tile DeadBush;
    public RuleTile Cracked;
    public RuleTile Purple;
}
