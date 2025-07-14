using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Switcher : MonoBehaviour, IInteractible
{
    [SerializeField] Transform _trap = default;
    [SerializeField] Tilemap _tilemap = default;
    [SerializeField] TileBase _tile = default;
    [SerializeField] GameObject _on = default;
    [SerializeField] GameObject _off = default;
    private Vector3Int cellPosition = default;
    public void Start()
    {
        _off.SetActive(false);
        _on.SetActive(true);
        cellPosition = _tilemap.WorldToCell(_trap.position);
        _tilemap.SetTile(cellPosition, null);
    }
    public void OnInteract()
    {
        _off.SetActive(true);
        _on.SetActive(false);
        _tilemap.SetTile(cellPosition, _tile);
    }
}
