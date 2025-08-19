using UnityEngine;
using UnityEngine.Tilemaps;


// The game physics and movement are based on 4 tilesets which need to be accesed from some classes, this struct handles all of them

[System.Serializable]
public struct GameTilemaps
{
    public Tilemap floorMap; // The tiles where the slug can move
    public Tilemap slugFace; // The slug face tilemap, since the face needs to change position between head and tail
    public Tilemap interactiblesMap; // These are scripted tiles
    public Tilemap slugMap; // The tilemap of the slug body
}
