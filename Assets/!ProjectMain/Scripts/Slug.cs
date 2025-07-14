
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Slug : MonoBehaviour
{
    [SerializeField] private Tilemap floor;
    [SerializeField] private Tilemap _slugMap = default;
    [SerializeField] private SlugTiles _slugTiles = default;
    [SerializeField] private Transform _boxCollider = default;
    private List<Vector3Int> _slugBody = default;
    [SerializeField] private Transform _startPos = default;
    private Vector3Int headPosition = default;
    [SerializeField] private Vector3Int direction = default;
    private bool _canMove = false;
    private bool _hasCollided = false;

    private CameraManager _mainCamera = default;
    private GameManager _gameManager = default;

    [Header("Sounds")]
    [SerializeField] private AudioClip _slide = default;
    [SerializeField] private AudioClip _collision = default;
    private void Start()
    {
        _gameManager = GameManager.Instance;
        _mainCamera = _gameManager.mainCamera;
        headPosition = new Vector3Int((int)_startPos.position.x, (int)_startPos.position.y, (int)_startPos.position.z);
        _slugBody = new List<Vector3Int>();
        _slugBody.Add(headPosition);
        _slugMap.SetTile(headPosition, _slugTiles.GetTile(headPosition));
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
        Vector3Int newHeadPosition;
        while (!_hasCollided)
        {
            newHeadPosition = headPosition + direction;
            if (floor.HasTile(newHeadPosition) && !_slugMap.HasTile(newHeadPosition))
            {
                _gameManager.mainAudioSource.clip = _slide;
                _gameManager.mainAudioSource.Play();
                _slugBody.Insert(0, newHeadPosition);
                _slugMap.SetTile(newHeadPosition, _slugTiles.GetTile(direction));


                for (int i = 1; i < _slugBody.Count; i++)
                {
                    Vector3Int bodySegment = _slugBody[i];
                    Vector3Int nextPosition = _slugBody[i - 1];
                    _slugMap.SetTile(bodySegment, _slugTiles.GetTile(bodySegment, nextPosition));
                }


                Vector3Int lastPosition = _slugBody[_slugBody.Count - 1];
                _slugMap.SetTile(lastPosition, _slugTiles.GetTile(headPosition, newHeadPosition));


                headPosition = newHeadPosition;
                yield return new WaitForSeconds(0.06f);
            }
            else
            {
                _hasCollided = true;
                _canMove = true;
                _mainCamera.ShakeCamera();
                _gameManager.mainAudioSource.clip = _collision;
                _gameManager.mainAudioSource.Play();
                _boxCollider.transform.position = headPosition;
            }
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

        Vector3Int lastSegment = _slugBody[_slugBody.Count - 1];
        Vector3Int firstSegment = _slugBody[0];
        _slugBody.Insert(0, lastSegment);
        _slugBody.Insert(_slugBody.Count, firstSegment);

        _slugMap.SetTile(lastSegment, _slugTiles.GetTile(direction));
        _slugMap.SetTile(firstSegment, _slugTiles.GetTile(firstSegment, _slugBody[_slugBody.Count - 2]));

        //  for (int i = 1; i < _slugBody.Count; i++)
        //  {
        //     Vector3Int bodySegment = _slugBody[i];
        //      Vector3Int nextPosition = _slugBody[i - 1];
        //      _slugMap.SetTile(bodySegment, _slugTiles.GetTile(bodySegment, nextPosition));
        //   }
        headPosition = _slugBody[0];
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<IInteractible>() != null)
        {          
            collision.collider.GetComponent<IInteractible>().OnInteract();
        }
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

        public TileBase GetTile(Vector3Int headDirection)
        {
            switch (headDirection)
            {
                case Vector3Int direction when headDirection == Vector3Int.up:
                    return headUp;
                case Vector3Int direction when headDirection == Vector3Int.down:
                    return headDown;
                case Vector3Int direction when headDirection == Vector3Int.left:
                    return headLeft;
                case Vector3Int direction when headDirection == Vector3Int.right:
                    return headRight;
                default:
                    return headDown;
            }
        }
    }
}
