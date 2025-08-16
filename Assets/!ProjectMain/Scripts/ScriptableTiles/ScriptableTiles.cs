using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class ScriptableTile : Tile
{

    protected Tilemap map;
    protected Vector3Int position;
    public virtual void BeginPlay(Tilemap inMap, Vector3Int cellPosition)
      {
        map = inMap;
        position = cellPosition;
      }
      public virtual void Interact()
      {

      }
}


[CreateAssetMenu(fileName = "StartPositionTile", menuName = "Tiles/StartPositionTile")]
public class StartPosition : ScriptableTile
{
    public override void BeginPlay(Tilemap inMap, Vector3Int cellPosition)
    {
        base.BeginPlay(inMap, cellPosition);

        GameManager.Instance.currentLevel.startPos = position;
    }
}

[CreateAssetMenu(fileName = "DoorTile", menuName = "Tiles/DoorTile")]
public class DoorTile : ScriptableTile
{
    public Action _activatation = default;
    public override void BeginPlay(Tilemap inMap, Vector3Int cellPosition)
    {
        base.BeginPlay(inMap, cellPosition);

        _activatation += GameManager.Instance.NextLevel;
    }

    public override void Interact()
    {
        _activatation?.Invoke();
    }
}