using System;
using UnityBase.Command;
using UnityEngine;
using VContainer;

public class MovementController : MonoBehaviour, IMoveEntity
{
    [Inject] 
    private readonly ICommandRecorder _commandRecorder;
    
    [SerializeField] private Transform _meshHandlerTransform;
    
    [SerializeField] private float _movementSpeed;

    private Vector3 _targetPosition;
    
    private ICommand _moveCommand;

    private bool _isInProgress;

    public Transform Transform => transform;
    public Transform MeshHandlerTransform => _meshHandlerTransform;
    public Vector3 TargetPosition => _targetPosition;
    public float Speed => _movementSpeed;
    public bool CanPassNextMovementInstantly => true;
    public bool IsMovementInProgress => _moveCommand?.IsInProgress ?? false;
    
    public void MoveBall(Vector3 targetPosition, Action onComplete)
    {
        _targetPosition = targetPosition;
        
        _moveCommand = MoveCommand.Create<ObjectMoveCommand>(this);
        
        _commandRecorder.Execute(_moveCommand, onComplete);
    }

    public void Dispose() => _commandRecorder.Dispose();
}