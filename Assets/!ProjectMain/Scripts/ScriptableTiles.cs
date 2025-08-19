using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;


/* This is the base class for all the scriptable tiles, since our game doesnt rely on physical collisions, we need to code inside
tilemaps and call Interact() to simulate a collision and get a reaction */
public abstract class ScriptableTile : Tile
{    
    protected Tilemap map; // The base tilemap where this tile is set
    protected Vector3Int position; // The position in the grid where the tile is set
    public bool _isRigid; // if true, the tile will stop the slug

    // Called before starting the level
    public virtual void BeginPlay(Tilemap inMap, Vector3Int cellPosition)
      {
        map = inMap;
        position = cellPosition;
      }

    // Called when the slug collides with the tile
      public virtual void Interact()
      {

      }
}


[CreateAssetMenu(fileName = "StartPositionTile", menuName = "Tiles/StartPositionTile")]
public class StartPosition : ScriptableTile // Set this tile wherever you want the slug to start
{
    public override void BeginPlay(Tilemap inMap, Vector3Int cellPosition)
    {
        base.BeginPlay(inMap, cellPosition);

        GameManager.Instance.currentLevel.startPos = position;
    }
}

[CreateAssetMenu(fileName = "DoorTile", menuName = "Tiles/DoorTile")]
public class DoorTile : ScriptableTile // This tile executes the finish of the level
{
    public override void BeginPlay(Tilemap inMap, Vector3Int cellPosition)
    {
        base.BeginPlay(inMap, cellPosition);
    }

    public override void Interact()
    {
        GameManager.Instance.NextLevel();
    }
}

[CreateAssetMenu(fileName = "ButtonTile", menuName = "Tiles/ButtonTile")]
public class ButtonTile : ScriptableTile // This is a trigger, when collided, it calls an event. YOU MUST CREATE ONE ASSETS FOR EACH INSTANCE SO YOU CAN CHANGE THE PROPERTIES.
{
    [SerializeField] private Sprite activated; 
    [SerializeField] private Sprite deactivated;

    [Tooltip("Set an event ID here, and then set this same ID to the asset you want to trigger")]
    [SerializeField] private string _eventID;
    public override void BeginPlay(Tilemap inMap, Vector3Int cellPosition)
    {
        base.BeginPlay(inMap, cellPosition);

        sprite = deactivated; // Set the sprite
    }
    public override void Interact()
    {
        GameManager.Instance.currentLevel.eventManager.Invoke(_eventID);

        sprite = activated; // Set activated sprite
    }
}

[CreateAssetMenu(fileName = "LockTile", menuName = "Tiles/LockTile")]
public class LockTile : ScriptableTile // This is a tile that dissapears when triggerd. YOU MUST CREATE ONE ASSETS FOR EACH INSTANCE SO YOU CAN CHANGE THE PROPERTIES.
{
    [Tooltip("Set here an event ID, then set this same event ID to a trigger tile")]
    [SerializeField] private string _eventID;
    public override void BeginPlay(Tilemap inMap, Vector3Int cellPosition)
    {
        base.BeginPlay(inMap, cellPosition);
        GameManager.Instance.currentLevel.eventManager.Subscribe(_eventID, Unlock);
    }
    public void Unlock()
    {
        map.SetTile(position, null); // Clear the tile
    }

}