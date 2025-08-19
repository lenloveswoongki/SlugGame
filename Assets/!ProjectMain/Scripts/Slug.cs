
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Slug : MonoBehaviour // This class handles the player spawn, movement and collision
{
    // body data
    [SerializeField] private TileBase _slugFaceTile;
    [SerializeField] private SlugTiles _slugTiles;
    private List<Vector3Int> _slugBody; // This list includes all the segments of the slug body
    private Vector3Int _headPosition; // The current position of the head, remember it switches positions with the tail

    // collision and movement
    private bool _canMove = false;
    private bool _hasCollided = false;
    [SerializeField] private Vector3Int _direction; // The current movement direction

    // gameplay data
    private GameTilemaps _maps;
    private CameraManager _mainCamera;
    private GameManager _gameManager;

    [Header("Sounds")]
    [SerializeField] private AudioClip _slide;
    [SerializeField] private AudioClip _collision;

    #region unity methods
    private void Start()
    {
        _gameManager = GameManager.Instance;
        _gameManager.currentLevel.player = this;
    }

    private void Update()
    {
        Move();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ReverseSnake();
        }
    } 
    #endregion

    public void BeginPlay(Vector3Int startPos) // called when current level its all set
    {    
        _mainCamera = _gameManager.mainCamera;

        // Spawn body
        _headPosition = new Vector3Int((int)startPos.x, (int)startPos.y, (int)startPos.z);
        _slugBody = new List<Vector3Int>(); 
        _slugBody.Add(_headPosition);
        SpawnHead();
    }

    public void SpawnHead() // When spawned, the slug has 2 segments, head and tile. So we have to calculate where the head and tail will be having the walls in mind
    {
        Vector3Int[] positions = new Vector3Int[]
        {
            Vector3Int.up + _headPosition,
            Vector3Int.down + _headPosition,
            Vector3Int.left + _headPosition,
            Vector3Int.right + _headPosition
        };

        foreach (Vector3Int i in positions)
        {

            if (_maps.floorMap.HasTile(i)) // if there is floor on "i" direction, we apwn the tail there
            {
                _maps.slugMap.SetTile(_headPosition, _slugTiles.GetHeadTile(i, _headPosition));
                _maps.slugFace.SetTile(_headPosition, _slugFaceTile);
                _maps.slugMap.SetTile(i, _slugTiles.GetHeadTile(_headPosition, i));
                _slugBody.Add(i);
                return;
            }
        }
    }

   
    private void Move()
    {
        _direction = GetDirection();
        _hasCollided = false;

        if (!_hasCollided && _direction != Vector3Int.zero && _canMove)
        {
            _hasCollided = false;
            _canMove = false;
            StartCoroutine(MovementCorroutine());
        }
    }

    private IEnumerator MovementCorroutine()
    {

        Vector3Int targetPosition; // The grid position where the slug will go

        while (true)
        {
            targetPosition = _headPosition + _direction;  // we generate a new position to expand our body

            HandleCollision(targetPosition);

            if (_hasCollided) // if has collided, stop the coroutine
            {
                yield break;
            }

            _maps.slugMap.SetTile(_headPosition, _slugTiles.GetTile(_headPosition, targetPosition)); // set the body tile in the previous head position

            _slugBody.Insert(0, targetPosition); // we add the new position to the segments list
            _maps.slugFace.ClearAllTiles(); // clear the face tiles
            _maps.slugMap.SetTile(targetPosition, _slugTiles.GetHeadTile(_headPosition, targetPosition)); // set the head in the new position

            _headPosition = targetPosition; 
            _maps.slugFace.SetTile(_headPosition, _slugFaceTile); // set the face in the new position

            yield return new WaitForSeconds(0.06f);
        }
    }


    private void HandleCollision(Vector3Int targetPosition) // this method handles a custom tiles collsion
    {
        if (_maps.interactiblesMap.HasTile(targetPosition)) // first we check if there is a interactible tile
        {
            ScriptableTile targetTile = _maps.interactiblesMap.GetTile<ScriptableTile>(targetPosition);
            targetTile.Interact();

            if(targetTile._isRigid) // if tile is rigid, we stop moving
                {
                _hasCollided = true;
                _canMove = true;
                _mainCamera.ShakeCamera();
                _gameManager.mainAudioSource.clip = _collision;
                _gameManager.mainAudioSource.Play();
                return;
            }
        }
        if (!_maps.floorMap.HasTile(targetPosition) || _maps.slugMap.HasTile(targetPosition)) // then we check if theres no more floor, or there is another segment
        {
            _hasCollided = true;
            _canMove = true;
            _mainCamera.ShakeCamera();
            _gameManager.mainAudioSource.clip = _collision;
            _gameManager.mainAudioSource.Play();
        }
    }
    private Vector3Int GetDirection() // here we get the direction by inputs, pretty straightforward
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            _canMove = true;
            return Vector3Int.up;

        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            _canMove = true;
            return Vector3Int.down;

        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            _canMove = true;
            return Vector3Int.left;

        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _canMove = true;
            return Vector3Int.right;

        }
        else
        {
            _canMove = false;
            return _direction;

        }
    }

    private void ReverseSnake() // this method reverses our snake, head goes to tail and backwards
    {
        _maps.slugFace.ClearAllTiles();
        _slugBody.Reverse();

        // We adjust some tiles
        _maps.slugMap.SetTile(_slugBody[0], _slugTiles.GetHeadTile(_slugBody[1], _slugBody[0]));
        _maps.slugMap.SetTile(_slugBody[_slugBody.Count - 1], _slugTiles.GetHeadTile(_slugBody[_slugBody.Count - 2], _slugBody[_slugBody.Count - 1]));
        _maps.slugFace.SetTile(_slugBody[0], _slugFaceTile);
        _headPosition = _slugBody[0];
    }

    
    public void SetTileMaps(GameTilemaps mapsIn) // call this to set tilemaps
    {
        _maps = mapsIn;
    }    
}

[System.Serializable]
public class SlugTiles // this class handles the slug tiles
{

    public TileBase headUp;
    public TileBase headDown;
    public TileBase headLeft;
    public TileBase headRight;
    public TileBase BodyVertical;
    public TileBase BodyHorizontal;


    public TileBase GetTile(Vector3Int position, Vector3Int nextPosition) // Returns a tile based on position and direction
    {

        if (position.y == nextPosition.y)
        {
            return BodyHorizontal;
        }
        else
        {
            return BodyVertical;
        }
    }

    public TileBase GetHeadTile(Vector3Int neckPos, Vector3Int targetHeadDirection) // returns a fixed head tile based on body direction on position
    {

        Vector3Int dir = targetHeadDirection - neckPos;

        switch (dir)
        {
            case Vector3Int direction when dir == Vector3Int.up:
                return headUp;
            case Vector3Int direction when dir == Vector3Int.down:
                return headDown;
            case Vector3Int direction when dir == Vector3Int.left:
                return headLeft;
            case Vector3Int direction when dir == Vector3Int.right:
                return headRight;
            default:
                return headDown;
        }
    }
}
