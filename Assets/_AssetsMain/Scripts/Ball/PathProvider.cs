using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PathProvider : MonoBehaviour
{
    [ReadOnly] [SerializeField] private int _lastTileIndex;

    [ReadOnly] [SerializeField] private Grid<TileBase> _activeGridData;

    public void SetGridData(Grid<TileBase> gridData) => _activeGridData = gridData;
    public Grid<TileBase> GetGridData => _activeGridData;
    
    public void SetLastTileIndex(int gridIndex) => _lastTileIndex = gridIndex;
    public int GetLastTileIndex => _lastTileIndex;

    public IList<TileObject> GetTilePath(Direction direction)
    {
        IList<TileObject> tileObjects = new List<TileObject>();
        
        var neighbour = _activeGridData.GetNeighbour(_lastTileIndex, direction);

        while (neighbour is TileObject tileObject)
        {
            neighbour = _activeGridData.GetNeighbour(tileObject.index, direction);

            _lastTileIndex = tileObject.index;

            tileObjects.Add(tileObject);
        }

        return tileObjects;
    }
}