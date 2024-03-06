using System;
using System.Linq;
using UnityBase.Pool;
using UnityBase.Service;
using UnityEngine;
using VContainer;

public class BallController : MonoBehaviour, IPoolable, IActivatable, IResettable
{
    [Inject] 
    private readonly IPoolDataService _poolDataService;
    
    [Inject]
    private readonly ISwipeInputService _swipeInputService;

    [SerializeField] private PathProvider _pathProvider;
    [SerializeField] private MovementController _movementController;
    [SerializeField] private MaterialProvider _materialProvider;
    [SerializeField] private GameObject _trailObject, _blastParticle;
    
    private TileVisitor _tileVisitor;
    private PaintCompleteChecker _paintCompleteChecker = new PaintCompleteChecker();
    private bool _isInputEnabled;

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
    }

    private void Update()
    {
        if (!_isInputEnabled)
        {
            _swipeInputService.ResetInput();
            return;
        }

        Direction direction = _swipeInputService.GetSwipeDirection();
        
        if (direction != Direction.None && !_movementController.IsMovementInProgress)
        {
            var tilePath = _pathProvider.GetTilePath(direction);
            
            if (tilePath.Count > 0)
            {
                var lastTile = tilePath.LastOrDefault();
                
                if (lastTile)
                {
                    _blastParticle.SetActive(false);

                    _movementController.MoveBall(lastTile.transform.position, OnMovementComplete);
                    
                    _tileVisitor.VisitTilePath(tilePath, direction, OnVisitComplete);
                }
            }
        }
    }

    private void OnMovementComplete() => _blastParticle.SetActive(true);

    private void OnVisitComplete()
    {
        var isAllTilesPainted = _paintCompleteChecker.IsAllTilesPainted(_pathProvider.GetGridData);

        if (isAllTilesPainted)
        {
            ActivateInitials(false);

            _movementController.Dispose();

            GridManager.OnAllTilesPainted?.Invoke();
        }
    }
    public void ActivateInitials(bool enable)
    {
        _isInputEnabled = enable;
        
        _trailObject.SetActive(enable);
    }
    
    public void Reset()
    {
        ActivateInitials(false);
        
        _blastParticle.SetActive(false);
    }

    private void OnDestroy()
    {
        _tileVisitor.Dispose();
        _poolDataService.Remove(this);
    }
}