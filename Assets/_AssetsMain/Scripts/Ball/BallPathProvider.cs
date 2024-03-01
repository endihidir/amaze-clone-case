using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class BallPathProvider : MonoBehaviour
{
    [SerializeField] private BallMaterialProvider _ballMaterialProvider;
    
    [ReadOnly] [SerializeField] private int _lastTileIndex;

    [ReadOnly] [SerializeField] private Grid<TileBase> _activeGridData;

    public int LastTileIndex => _lastTileIndex;
    
    public void SetGridData(Grid<TileBase> gridData) => _activeGridData = gridData;
    public void SetTileIndex(int gridIndex) => _lastTileIndex = gridIndex;

    public IList<TileObject> GetTilePath(Direction direction, float movementSpeed)
    {
        IList<TileObject> tileObjects = new List<TileObject>();
        
        var neighbour = _activeGridData.GetNeighbour(_lastTileIndex, direction);

        while (neighbour is TileObject tileObject)
        {
            neighbour = _activeGridData.GetNeighbour(tileObject.index, direction);

            _lastTileIndex = tileObject.index;

            tileObjects.Add(tileObject);
        }
        
        StartCoroutine(SetTileColor(tileObjects, movementSpeed));

        return tileObjects;
    }

    public TileObject GetLastTileObject(Direction direction, float movementSpeed) => GetTilePath(direction, movementSpeed).LastOrDefault();

    private IEnumerator SetTileColor(IList<TileObject> tileObjects, float movementSpeed)
    {
        foreach (var tilObject in tileObjects)
        {
            tilObject.SetMaterial(_ballMaterialProvider.CurrentStampMaterial);
            var paintSpeed = (movementSpeed / tileObjects.Count) * 0.004f;
            yield return new WaitForSeconds(paintSpeed);
        }
        
        DetectAllTilePainted();
    }


    private void DetectAllTilePainted()
    {
        bool isAllPainted = false;
        
        for (int x = 0; x < _activeGridData.Width; x++)
        {
            for (int z = 0; z < _activeGridData.Height; z++)
            {
                var tile = _activeGridData.GetGridObject(x, z);

                if (tile is TileObject tileObject)
                {
                    isAllPainted = tileObject.IsPainted;
                    
                    if (!isAllPainted) return;
                }
            }
        }

        if (isAllPainted)
        {
            GridManager.OnAllTilesPainted?.Invoke();
        }
    }

}