using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public float timer = default;
    public Text timerText = default;
    public Tilemap _scriptableTiles;
    [SerializeField] private Slug player;

    public Vector3Int startPos;
    public void Awake()
    {
        GameManager gm = GameManager.Instance;
        gm.timer = timer;
        gm._timerText = timerText;
        gm.currentLevel = this;
        Initialize();
        gm.OnLevelStarts();
    }

    private void Start()
    {
        player.BeginPlay(startPos);
    }
    private void Initialize()
    {
        BoundsInt tilemmapBounds = _scriptableTiles.cellBounds;

        foreach(Vector3Int i in tilemmapBounds.allPositionsWithin)
        {
            ScriptableTile tile = _scriptableTiles.GetTile<ScriptableTile>(i);

            if(tile != null )
            {
                tile.BeginPlay(_scriptableTiles, i);
            }
        }
    }
}

