using Sirenix.OdinInspector;
using UnityEngine;
using UnityBase.GridSystem;

namespace UnityBase.PathFinding
{
    public class PathFindingTest : MonoBehaviour
    {
        #region VARIABLES

        [SerializeField] private Vector2Int _gridSize;

        [SerializeField] private float _cellSize;

        [ReadOnly] [SerializeField] private Vector3 _centerPos;

        private GridXY<PathNode> _gridXY;

        private PathNode _startNode;

        private PathFinding _pathFinding;

        #endregion

        #region COMPONENTS

        private Camera _cam;

        private Mesh _mesh;

        #endregion

        #region PROPERTIES

        private int Width => _gridSize.x;
        private int Height => _gridSize.y;

        #endregion

        private void Awake()
        {
            _mesh = new Mesh();

            GetComponent<MeshFilter>().mesh = _mesh;
        }

        private void Start()
        {
            _cam = Camera.main;

            _centerPos = new Vector3(-Width * _cellSize, -Height * _cellSize) * 0.5f;

            _pathFinding = new PathFinding(Width, Height, _cellSize, _centerPos, transform);

            _gridXY = _pathFinding.GetGrid();

            _startNode = _gridXY.GetGridObject(_centerPos);
        }

        private void Update()
        {
            var mouseWorldPos = _cam.ScreenToWorldPoint(Input.mousePosition);

            mouseWorldPos.z = 0f;

            if (Input.GetMouseButtonDown(0))
            {
                if (_gridXY == null) return;

                _gridXY.GetXY(mouseWorldPos, out var x, out var y);

                if (!_gridXY.IsInRange(x, y)) return;

                var endNode = _gridXY.GetGridObject(mouseWorldPos);

                if (!endNode.isWalkable || endNode.index == _startNode.index) return;

                var path = _pathFinding.FindPath(_startNode.Pos, endNode.Pos);

                _startNode = endNode;

                var startX = _centerPos.x / _cellSize;
                var startY = _centerPos.y / _cellSize;

                if (path.Length > 0)
                {
                    for (int i = 0; i < path.Length - 1; i++)
                    {
                        var startPos = new Vector3(startX + path[i].x, startY + path[i].y) * _cellSize +
                                       Vector3.one * (_cellSize * 0.5f);

                        var endPos = new Vector3(startX + path[i + 1].x, startY + path[i + 1].y) * _cellSize +
                                     Vector3.one * (_cellSize * 0.5f);

                        Debug.DrawLine(startPos, endPos, Color.green, 3f);
                    }
                }

                path.Dispose();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (_gridXY == null) return;

                var pathNode = _gridXY.GetGridObject(mouseWorldPos);

                pathNode.SetWalkable(!pathNode.isWalkable);

                _gridXY.SetGridObject(mouseWorldPos, pathNode);

                UpdateVisual();
            }
        }

        private void UpdateVisual()
        {
            MeshUtils.CreateEmptyMeshArrays(Width * Height, out var vertices, out var uv, out var triangles);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var grid = _gridXY.GetGridObject(x, y);

                    var quadSize = grid.isWalkable ? Vector3.zero : new Vector3(1, 1) * _cellSize;

                    var pos = _gridXY.GetWorldPosition(x, y) + quadSize * 0.5f;

                    MeshUtils.AddToMeshArrays(vertices, uv, triangles, grid.index, pos, 0f, quadSize, Vector2.zero,
                        Vector2.zero);
                }
            }

            _mesh.vertices = vertices;
            _mesh.uv = uv;
            _mesh.triangles = triangles;
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            if (_gridXY == null) return;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Gizmos.DrawLine(_gridXY.GetWorldPosition(x, y), _gridXY.GetWorldPosition(x, y + 1));
                    Gizmos.DrawLine(_gridXY.GetWorldPosition(x, y), _gridXY.GetWorldPosition(x + 1, y));
                }

                Gizmos.DrawLine(_gridXY.GetWorldPosition(0, Height), _gridXY.GetWorldPosition(Width, Height));
                Gizmos.DrawLine(_gridXY.GetWorldPosition(Width, 0), _gridXY.GetWorldPosition(Width, Height));
            }
        }
    }
}