using UnityEngine;

public class TilemapsManager : MonoBehaviour // This class must be attached to an object and from inspector you must set the 4 tilemaps
{
    public GameTilemaps tilemaps;

    public void Start()
    {
        GameManager.Instance.currentLevel.OnReady += SetTileMaps;
    }

    public void SetTileMaps() // Set the tilemaps on the player
    {
        GameManager.Instance.currentLevel.player.SetTileMaps(tilemaps);
        InitializeTilemaps();
    }

    public void InitializeTilemaps() // This method calls the BeginPlay() in interactibles tiles
    {
        BoundsInt tilemmapBounds = tilemaps.interactiblesMap.cellBounds;

        foreach (Vector3Int i in tilemmapBounds.allPositionsWithin)
        {
            ScriptableTile tile = tilemaps.interactiblesMap.GetTile<ScriptableTile>(i);

            if (tile != null)
            {
                tile.BeginPlay(tilemaps.interactiblesMap, i);
            }
        }
    }
}
