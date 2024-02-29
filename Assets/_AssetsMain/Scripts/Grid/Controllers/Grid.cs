using Unity.Mathematics;
using UnityEngine;

public class Grid<T> where T : GridNode
{
    #region VARIABLES

    private IGridEntity _gridEntity;

    private T[,] _gridArray;
    
    #endregion

    #region PROPERTIES
    
    public int Width => _gridEntity.Width;
    public int Height => _gridEntity.Height;
    public float NodeSize => _gridEntity.NodeSize;
    public float3 Padding => _gridEntity.Padding;
    public T[,] GridArray => _gridArray;

    #endregion

    public Grid(IGridEntity gridEntity)
    {
        _gridEntity = gridEntity;
        
        _gridArray = new T[_gridEntity.Width, _gridEntity.Height];
    }

    ~Grid() => _gridEntity = null;
    
    public float3 GetWorldPosition(int x, int y, int z)
    {
        var xPos = x * (_gridEntity.NodeSize + _gridEntity.Padding.x);
        var zPos = z * (_gridEntity.NodeSize + _gridEntity.Padding.z);
        
        var worldPos = new float3(xPos , y, zPos) + _gridEntity.OriginPos;
        
        return worldPos;
    }

    public T GetGridObject(float3 worldPos)
    {
        GetXZ(worldPos, out var x, out var z);

        var gridObj = GetGridObject(x, z);
        
        return gridObj;
    }
    
    public T GetGridObject(int x, int z)
    {
        if (!IsInRange(x, z)) return null;

        return _gridArray[x, z];
    }
    
    public void SetGridObject(float3 worldPos, T value)
    {
        GetXZ(worldPos, out var x, out var z);
        
        if(!IsInRange(x, z)) return;

        _gridArray[x, z] = value;
    }
    
    public void SetGridObject(int x, int z, T gridNode)
    {
        if(_gridArray.GetLength(0) < x || _gridArray.GetLength(1) < z) return;
        
        _gridArray[x, z] = gridNode;
    }

    public bool IsInRange(int x, int z)
    {
        bool isInRange = x >= 0 && z >= 0 && x < _gridEntity.Width && z < _gridEntity.Height;
        
        return isInRange;
    }

    public void GetXZ(float3 worldPos, out int x, out int z)
    {
        var absolutePos = worldPos - _gridEntity.OriginPos;

        var xDivider = _gridEntity.NodeSize + _gridEntity.Padding.x;
        var zDivider = _gridEntity.NodeSize + _gridEntity.Padding.z;
        
        x = Mathf.FloorToInt(absolutePos.x / xDivider);
        z = Mathf.FloorToInt(absolutePos.z / zDivider);
    }

    public int CalculateIndex(int x, int z)
    {
        var val = (z * _gridEntity.Width) + x;
        return val;
    }
}