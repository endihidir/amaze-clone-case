using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityBase.Extensions;
using UnityBase.Visitor;

public class TileVisitor : IVisitor
{
    private readonly MovementController _movementController;
    private readonly MaterialProvider _materialProvider;
    private readonly PathProvider _pathProvider;
    
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public TileVisitor(MovementController movementController, MaterialProvider materialProvider, PathProvider pathProvider)
    {
        _movementController = movementController;
        _materialProvider = materialProvider;
        _pathProvider = pathProvider;
    }
    
    public async void VisitTilePath(IList<TileObject> tilePath, Direction direction, Action onComplete)
    {
        CancellationTokenExtentions.Refresh(ref _cancellationTokenSource);

        var gridData = _pathProvider.GetGridData;
        
        var delay = (1f / _movementController.Speed) / ((gridData.NodeSize + gridData.Padding.x) * 1.5f);
        
        try
        {
            foreach (var tileObject in tilePath)
            {
                await UniTask.WaitForSeconds(delay, false, PlayerLoopTiming.Update, _cancellationTokenSource.Token);
            
                tileObject.SetMaterial(_materialProvider.CurrentStampMaterial);
                
                tileObject.PlayTileAnim(direction);
                
                Visit(tileObject);
            }
        
            onComplete?.Invoke();
        }
        catch (Exception e)
        {
            //Debug.Log(e);
        }
    }

    public void Visit<T>(T visitable) where T : IVisitable
    {
        if (visitable is CoinTileObject coinTileObject)
        {
            coinTileObject.CollectCoin();
            
            GridManager.OnCollectCoinTile?.Invoke(coinTileObject);
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
    }
}