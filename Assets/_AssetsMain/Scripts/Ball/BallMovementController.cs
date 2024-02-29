using System;
using UnityBase.Command;
using UnityBase.Pool;
using UnityBase.Service;
using UnityEngine;
using VContainer;

public class BallMovementController : MonoBehaviour, IPoolable, IMoveEntity
{
    [Inject] 
    private readonly ICommandDataService _commandDataService;

    [Inject] 
    private readonly ICommandRecorder _commandRecorder;
    
    [Inject] 
    private readonly IPoolDataService _poolDataService;

    [Inject]
    private readonly ISwipeInputService _swipeInputService;

    [SerializeField] private float _movementDuration;

    [SerializeField] private bool _canPassNextMovementInstantly;

    private SwipeDirection _previousSwipeDirection;
    
    private Vector3 _targetPosition;
    
    private ICommand _moveCommand;
    public Component PoolableObject => this;
    public bool IsActive => isActiveAndEnabled;
    public bool IsUnique => false;
    public Transform Transform => transform;
    public Vector3 NewPosition => _targetPosition;
    public float Duration => _movementDuration;
    public bool CanPassNextMovementInstantly => _canPassNextMovementInstantly;
    
    public void Show(float duration, float delay, Action onComplete)
    {
        gameObject.SetActive(true);
        onComplete?.Invoke();
    }

    public void Hide(float duration, float delay, Action onComplete)
    {
        gameObject.SetActive(false);
        onComplete?.Invoke();
    }

    private void Update()
    {
        SwipeDirection swipeDirection = _swipeInputService.GetSwipeDirection();
        
        if (swipeDirection != SwipeDirection.None)
        {
            Vector3 swipeVector = _swipeInputService.CastDirectionToVector(swipeDirection);
            
            _targetPosition = transform.position + (swipeVector * 2f);

            _moveCommand = MoveCommand.Create<ObjectMoveCommand>(this);

            _commandRecorder.Execute(_moveCommand);
        }
    }

    private void OnDestroy()
    {
        _poolDataService.Remove(this);
    }
}