
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Slug : MonoBehaviour
{
    [SerializeField] private GameTilemaps _maps;
    [SerializeField] private TileBase _slugFaceTile;
    [SerializeField] private SlugTiles _slugTiles = default;
    private List<Vector3Int> _slugBody = default;
    private Vector3Int headPosition = default;
    [SerializeField] private Vector3Int direction = default;
    private bool _canMove = false;
    private bool _hasCollided = false;

    private CameraManager _mainCamera = default;
    private GameManager _gameManager = default;

    [Header("Sounds")]
    [SerializeField] private AudioClip _slide = default;
    [SerializeField] private AudioClip _collision = default;
    public void BeginPlay(Vector3Int startPos)
    {
        _gameManager = GameManager.Instance;
        _mainCamera = _gameManager.mainCamera;
        headPosition = new Vector3Int((int)startPos.x, (int)startPos.y, (int)startPos.z);
        _slugBody = new List<Vector3Int>();
        _slugBody.Add(headPosition);
        SpawnHead();
    }

    public void SpawnHead()
    {
        Vector3Int[] positions = new Vector3Int[]
        {
            Vector3Int.up + headPosition,
            Vector3Int.down + headPosition,
            Vector3Int.left + headPosition,
            Vector3Int.right + headPosition
        };

        foreach (Vector3Int i in positions)
        {

            if (_maps.floorMap.HasTile(i))
            {
                _maps.slugMap.SetTile(headPosition, _slugTiles.GetHeadTile(i, headPosition));
                _maps.slugFace.SetTile(headPosition, _slugFaceTile);
                _maps.slugMap.SetTile(i, _slugTiles.GetHeadTile(headPosition, i));
                _slugBody.Add(i);
                return;
            }
        }
    }

    private void Update()
    {
        Move();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ReverseSnake();
        }

    }
    private void Move()
    {
        direction = GetDirection();
        _hasCollided = false;

        if (!_hasCollided && direction != Vector3Int.zero && _canMove)
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

            targetPosition = headPosition + direction;

            HandleCollision(targetPosition);
            if (_hasCollided)
            {
                yield break;
            }

            _maps.slugMap.SetTile(headPosition, _slugTiles.GetTile(headPosition, targetPosition));


            _slugBody.Insert(0, targetPosition);
            _maps.slugFace.ClearAllTiles();
            _maps.slugMap.SetTile(targetPosition, _slugTiles.GetHeadTile(headPosition, targetPosition));

            headPosition = targetPosition;
            _maps.slugFace.SetTile(headPosition, _slugFaceTile);
            yield return new WaitForSeconds(0.06f);
        }
    }


    private void HandleCollision(Vector3Int targetPosition)
    {
        if (!_maps.floorMap.HasTile(targetPosition) || _maps.slugMap.HasTile(targetPosition))
        {
            _hasCollided = true;
            _canMove = true;
            _mainCamera.ShakeCamera();
            _gameManager.mainAudioSource.clip = _collision;
            _gameManager.mainAudioSource.Play();
        }
        if (_maps.interactiblesMap.HasTile(targetPosition))
        {
            _maps.interactiblesMap.GetTile<ScriptableTile>(targetPosition).Interact();
        }
    }
    private Vector3Int GetDirection()
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
            return direction;

        }
    }

    private void ReverseSnake()
    {

        _maps.slugFace.ClearAllTiles();

        _slugBody.Reverse();
        _maps.slugMap.SetTile(_slugBody[0], _slugTiles.GetHeadTile(_slugBody[1], _slugBody[0]));
        _maps.slugMap.SetTile(_slugBody[_slugBody.Count - 1], _slugTiles.GetHeadTile(_slugBody[_slugBody.Count - 2], _slugBody[_slugBody.Count - 1]));
        _maps.slugFace.SetTile(_slugBody[0], _slugFaceTile);
        headPosition = _slugBody[0];
    }
    [System.Serializable]
    public class SlugTiles
    {

        public TileBase headUp;
        public TileBase headDown;
        public TileBase headLeft;
        public TileBase headRight;
        public TileBase BodyVertical;
        public TileBase BodyHorizontal;


        public TileBase GetTile(Vector3Int position, Vector3Int nextPosition)
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

        public TileBase GetHeadTile(Vector3Int neckPos, Vector3Int targetHeadDirection)
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
}
