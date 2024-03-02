using System;
using System.Linq;
using UnityBase.Pool;
using UnityBase.Service;
using UnityEngine;
using VContainer;

public class BallController : MonoBehaviour, IPoolable, IInputInitializable, IResettable
{
    [Inject] 
    private readonly IPoolDataService _poolDataService;
    
    [Inject]
    private readonly ISwipeInputService _swipeInputService;

    [SerializeField] private PathProvider _pathProvider;
    [SerializeField] private MovementController _movementController;
    [SerializeField] private MaterialProvider _materialProvider;
    
    private TileVisitor _tileVisitor;
    private PaintCompleteChecker _paintCompleteChecker = new PaintCompleteChecker();
    private bool _isInputEnabled, _isMovementInProgress;

    public Component PoolableObject => this;
    public bool IsActive => isActiveAndEnabled;
    public bool IsUnique => false;
    
    public PathProvider PathProvider => _pathProvider;
    public MaterialProvider MaterialProvider => _materialProvider;

    private void Awake() => _tileVisitor = new TileVisitor(_movementController, _materialProvider, _pathProvider);

    public void Show(float duration, float delay, Action onComplete)
    {
        gameObject.SetActive(true);
        _materialProvider.SetRandomMaterial();
        onComplete?.Invoke();
    }

    public void Hide(float duration, float delay, Action onComplete)
    {
        gameObject.SetActive(false);
        onComplete?.Invoke();
        transform.localPosition = Vector3.zero;
    }

    private void Update()
    {
        if (!_isInputEnabled)
        {
            _swipeInputService.ResetInput();
            return;
        }

        Direction direction = _swipeInputService.GetSwipeDirection();
        
        if (direction != Direction.None && !_isMovementInProgress)
        {
            var tilePath = _pathProvider.GetTilePath(direction);
            
            if (tilePath.Count > 0)
            {
                var lastTile = tilePath.LastOrDefault();
                
                if (lastTile)
                {
                    _isMovementInProgress = true;
                    
                    _movementController.MoveBall(lastTile.transform.position, () => _isMovementInProgress = false);
                    
                    _tileVisitor.VisitTilePath(tilePath, OnVisitComplete);
                }
            }
        }
    }

    private void OnVisitComplete()
    {
        var isAllTilesPainted = _paintCompleteChecker.IsAllTilesPainted(_pathProvider.GetGridData);

        if (isAllTilesPainted)
        {
            EnableInput(false);
            
            GridManager.OnAllTilesPainted?.Invoke();
        }
    }
    public void EnableInput(bool enable) => _isInputEnabled = enable;
    public void Reset() => EnableInput(false);

    private void OnDestroy()
    {
        _tileVisitor.Dispose();
        _poolDataService.Remove(this);
    }
}