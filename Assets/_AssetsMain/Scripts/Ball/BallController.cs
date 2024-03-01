using System;
using UnityBase.Command;
using UnityBase.Pool;
using UnityBase.Service;
using UnityEngine;
using VContainer;

public class BallController : MonoBehaviour, IPoolable, IMoveEntity, IInputInitializeable, IResettable
{
    [Inject] 
    private readonly ICommandRecorder _commandRecorder;
    [Inject] 
    private readonly IPoolDataService _poolDataService;
    [Inject]
    private readonly ISwipeInputService _swipeInputService;

    [SerializeField] private BallPathProvider _pathProvider;
    [SerializeField] private BallMaterialProvider _materialProvider;
    
    [SerializeField] private Transform _meshTransform;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private bool _canPassNextMovementInstantly;

    private Direction _previousDirection;
    private Vector3 _targetPosition;
    private ICommand _moveCommand;
    private bool _isInputEnabled;

    public Component PoolableObject => this;
    public bool IsActive => isActiveAndEnabled;
    public bool IsUnique => false;
    
    public Transform MeshTransform => _meshTransform;
    public Transform Transform => transform;
    public Vector3 NewPosition => _targetPosition;
    public float Speed => _movementSpeed;
    public bool CanPassNextMovementInstantly => _canPassNextMovementInstantly;
    public BallPathProvider PathProvider => _pathProvider;
    public BallMaterialProvider MaterialProvider => _materialProvider;

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
        if(!_isInputEnabled) return;
        
        if(_swipeInputService == null) return;
        
        Direction direction = _swipeInputService.GetSwipeDirection();
        
        if (direction != Direction.None)
        {
            var lastTile = _pathProvider.GetLastTileObject(direction, Speed);

            if (lastTile)
            {
                _targetPosition = lastTile.transform.position;

                _moveCommand = MoveCommand.Create<ObjectMoveCommand>(this);

                _commandRecorder.Execute(_moveCommand);
            }
        }
    }

    private void OnDestroy() => _poolDataService.Remove(this);
    public void EnableInput(bool enable) => _isInputEnabled = enable;
    public void Reset() => _isInputEnabled = false;
}