using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Scene Data/Level")]
public class Level : GameScene
{
    // Settings specific to level only
    [Header("Level specific")]
    public Tilemap tilemap;
}
