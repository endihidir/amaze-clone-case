using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityBase.GridSystem
{
    [Serializable]
    public class HexGrid<T> where T : class
    {
        private HexGridData<T> _hexGridData;
        public T[,] GridArray => _hexGridData.gridArray;
        public int Width => _hexGridData.width;
        public int Height => _hexGridData.height;
        public bool IsGridInitialized { get; set; }

        public HexGrid(HexGridData<T> hexGridData)
        {
            _hexGridData = hexGridData;
        }

        ~HexGrid()
        {
            _hexGridData = default;

            IsGridInitialized = false;
        }

        public void FillArray(int x, int z, T gridNode)
        {
            _hexGridData.gridArray[x, z] = gridNode;
        }

        public void FillArray(T[] gridArray)
        {
            _hexGridData.gridArray = new T[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int z = 0; z < Height; z++)
                {
                    var index = CalculateIndex(x, z, Height);

                    _hexGridData.gridArray[x, z] = gridArray[index];
                }
            }

            IsGridInitialized = true;
        }

        public Vector3 GetWorldPosition(int x, int y, int z)
        {
            var xSize = _hexGridData.cellSize.x;
            var zSize = _hexGridData.cellSize.y;

            var offsetMultiplier = _hexGridData.offsetMultiplier;

            if (_hexGridData.useVertical)
            {
                var xOffset = (z % 2) == 1 ? xSize * 0.5f : 0f;

                var xPos = (x * xSize + xOffset) * offsetMultiplier.x;

                var zPos = (z * zSize * offsetMultiplier.y);

                return new Vector3(xPos, y, zPos) + _hexGridData.originPosition;
            }
            else
            {
                var zOffset = (x % 2) == 1 ? zSize * 0.5f : 0f;

                var xPos = (x * xSize * offsetMultiplier.y);

                var zPos = (z * zSize + zOffset) * offsetMultiplier.x;

                return new Vector3(xPos, y, zPos) + _hexGridData.originPosition;
            }
        }

        public T GetGridObjectBy(Vector3 worldPos)
        {
            GetXZ(worldPos, out var x, out var z);

            var gridObj = GetGridObjectBy(x, z);

            return gridObj;
        }

        public void SetGridObject(Vector3 worldPos, T value)
        {
            GetXZ(worldPos, out var x, out var z);

            if (!IsInRange(x, z)) return;

            GetGridArray()[x, z] = value;
        }

        public T GetGridObjectBy(int xIndex, int zIndex)
        {
            if (!IsInRange(xIndex, zIndex)) return null;

            return GetGridArray()[xIndex, zIndex];
        }

        public bool IsInRange(int x, int z)
        {
            var isInRange = x >= 0 && z >= 0 && x < _hexGridData.width && z < _hexGridData.height;

            return isInRange;
        }

        private void GetXZ(Vector3 worldPos, out int x, out int z)
        {
            var absolutePos = worldPos - _hexGridData.originPosition;

            var xDivider = _hexGridData.cellSize.x;
            var zDivider = _hexGridData.cellSize.y;

            var offsetMultiplier = _hexGridData.offsetMultiplier;

            var roughX = Mathf.RoundToInt(absolutePos.x / xDivider /
                                          (_hexGridData.useVertical ? offsetMultiplier.x : offsetMultiplier.y));
            var roughZ = Mathf.RoundToInt(absolutePos.z / zDivider /
                                          (_hexGridData.useVertical ? offsetMultiplier.y : offsetMultiplier.x));

            var roughXZ = new Vector3Int(roughX, 0, roughZ);

            var neighbourXZList = GetNeighbours(roughXZ);

            var closestXZ = roughXZ;

            foreach (var neighbourXZ in neighbourXZList)
            {
                var dist = Vector3.Distance(worldPos, GetWorldPosition(neighbourXZ.x, 0, neighbourXZ.z));
                var closestDist = Vector3.Distance(worldPos, GetWorldPosition(closestXZ.x, 0, closestXZ.z));

                if (dist < closestDist)
                {
                    closestXZ = neighbourXZ;
                }
            }

            x = closestXZ.x;
            z = closestXZ.z;
        }

        private List<Vector3Int> GetNeighbours(Vector3Int roughXZ)
        {
            if (_hexGridData.useVertical)
            {
                var oddRow = roughXZ.z % 2 == 1;

                return new List<Vector3Int>()
                {
                    roughXZ + new Vector3Int(-1, 0, 0),
                    roughXZ + new Vector3Int(1, 0, 0),

                    roughXZ + new Vector3Int(oddRow ? 1 : -1, 0, 1),
                    roughXZ + new Vector3Int(0, 0, 1),

                    roughXZ + new Vector3Int(oddRow ? 1 : -1, 0, -1),
                    roughXZ + new Vector3Int(0, 0, -1),
                };
            }
            else
            {
                var oddColumn = roughXZ.x % 2 == 1;

                return new List<Vector3Int>()
                {
                    roughXZ + new Vector3Int(oddColumn ? 1 : -1, 0, 0),
                    roughXZ + new Vector3Int(0, 0, 1),

                    roughXZ + new Vector3Int(1, 0, oddColumn ? 1 : -1),
                    roughXZ + new Vector3Int(-1, 0, oddColumn ? 1 : -1),

                    roughXZ + new Vector3Int(0, 0, -1),
                    roughXZ + new Vector3Int(oddColumn ? -1 : 1, 0, 0),
                };
            }
        }

        public T[] GetObjectNeighbours(Vector3 worldPos)
        {
            GetXZ(worldPos, out var x, out var z);

            var neighbourXZList = GetNeighbours(new Vector3Int(x, 0, z));
            var neighbours = new List<T>();

            foreach (var neighbourXZ in neighbourXZList)
            {
                var neighbour = GetGridObjectBy(neighbourXZ.x, neighbourXZ.z);
                if (neighbour != null)
                {
                    neighbours.Add(neighbour);
                }
            }

            return neighbours.ToArray();
        }

        private T[,] GetGridArray()
        {
            return _hexGridData.gridArray;
        }

        public int CalculateIndex(int x, int z, int gridHeight)
        {
            var val = (x * gridHeight) + z;
            return val;
        }
    }

    [Serializable]
    public struct HexGridData<T> where T : class
    {
        public bool useVertical;
        public int width, height;
        public Vector2 cellSize;
        public Vector2 offsetMultiplier;
        public Vector3 originPosition;
        public T[,] gridArray;
    }
}