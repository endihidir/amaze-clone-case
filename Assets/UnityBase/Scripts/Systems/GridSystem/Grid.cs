using System;
using CodeMonkey.Utils;
using UnityEngine;

namespace UnityBase.GridSystem
{
    public class Grid<T> where T : struct
    {
        private int _width;
        private int _height;
        private float _cellSize;
        private Vector3 _originPos;
        private T[,] _gridArray;
        private TextMesh[,] _debugTextArray;
        public int Height => _height;
        public int Width => _width;

        public Grid(int width, int height, float cellSize, Vector3 originPos, Transform parent, Func<Grid<T>, int, int, T> createGridObject)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _originPos = originPos;
            _gridArray = new T[_width, _height];
            _debugTextArray = new TextMesh[_width, _height];

            SetDefaultGridData(createGridObject);
            FillGridsText(parent);
        }

        private void SetDefaultGridData(Func<Grid<T>, int, int, T> createGridObject)
        {
            for (int x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < _gridArray.GetLength(1); y++)
                {
                    _gridArray[x, y] = createGridObject(this, x, y);
                }
            }
        }
        
        private void FillGridsText(Transform parent)
        {
            for (int x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < _gridArray.GetLength(1); y++)
                {
                    var text = _gridArray[x, y].ToString();
                    var worldPos = GetWorldPosition(x, y) + new Vector3(_cellSize, _cellSize) * 0.5f;
                    _debugTextArray[x, y] = UtilsClass.CreateWorldText(text, parent, worldPos, (int)(_cellSize * 2), Color.white, TextAnchor.MiddleCenter);
                }
            }
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, y) * _cellSize + _originPos;
        }

        public T GetGridObject(Vector3 worldPos)
        {
            GetXY(worldPos, out var x, out var y);

            return GetGridObject(x, y);
        }

        public T GetGridObject(int x, int y)
        {
            return !IsInRange(x, y) ? default : _gridArray[x, y];
        }

        public void SetGridObject(Vector3 worldPos, T value)
        {
            GetXY(worldPos, out var x, out var y);

            if (!IsInRange(x, y)) return;

            _gridArray[x, y] = value;
        }

        public bool IsInRange(int x, int y)
        {
            return x >= 0 && y >= 0 && x < _width && y < _height;
        }

        public void GetXY(Vector3 worldPos, out int x, out int y)
        {
            var absolutePos = worldPos - _originPos;
            x = Mathf.FloorToInt(absolutePos.x / _cellSize);
            y = Mathf.FloorToInt(absolutePos.y / _cellSize);
        }
    }
}