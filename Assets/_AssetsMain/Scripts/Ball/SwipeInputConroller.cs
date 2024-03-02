using UnityBase.Service;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static PlayerInputActions;

public class SwipeInputConroller : ISwipeInputService, IGameplayPresenterDataService, IPlayerActions
{
    private bool _detectSwipeOnlyAfterRelease = false;
    private float _minDistanceForSwipe = Screen.width * 0.1f;
    
    private Vector2 _fingerDownPosition, _fingerUpPosition;
    private Direction _direction;

    private bool _isDragging;

    private readonly PlayerInputActions _playerInputActions;

    public SwipeInputConroller()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.SetCallbacks(this);
        _playerInputActions.Enable();
    }

    ~SwipeInputConroller() => _playerInputActions.Disable();

    public void Initialize() { }
    public void Start() { }
    public void Dispose() { }
    
    public Direction GetSwipeDirection()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            ResetInput();
            return _direction;
        }
        
        if (_playerInputActions.Player.MouseLeftButton.WasPressedThisFrame())
        {
            _isDragging = true;
            _fingerDownPosition = _playerInputActions.Player.MousePosition.ReadValue<Vector2>();
            _fingerUpPosition = _playerInputActions.Player.MousePosition.ReadValue<Vector2>();
        }

        if (_isDragging && _playerInputActions.Player.MouseLeftButton.IsPressed())
        {
            _fingerUpPosition = _playerInputActions.Player.MousePosition.ReadValue<Vector2>();
            CheckSwipe();
        }

        if (_playerInputActions.Player.MouseLeftButton.WasReleasedThisFrame())
        {
            _isDragging = false;
            _fingerUpPosition = _playerInputActions.Player.MousePosition.ReadValue<Vector2>();
            CheckSwipe();
        }

        return _direction;
    }

    private void CheckSwipe()
    {
        var deltaX = _fingerUpPosition.x - _fingerDownPosition.x;
        var deltaY = _fingerUpPosition.y - _fingerDownPosition.y;

        if (!(Mathf.Abs(deltaX) > _minDistanceForSwipe) && !(Mathf.Abs(deltaY) > _minDistanceForSwipe))
        {
            _direction = Direction.None;
            return;
        }
        
        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
        {
            _direction = deltaX > 0 ? Direction.Right : deltaX < 0 ? Direction.Left : Direction.None;
        }
        else
        {
            _direction = deltaY > 0 ? Direction.Up : deltaY < 0 ? Direction.Down : Direction.None;
        }

        _fingerDownPosition = _fingerUpPosition;
    }

    public void ResetInput()
    {
        _direction = Direction.None;
        _fingerDownPosition = Vector2.zero;
        _fingerUpPosition = Vector2.zero;
        _isDragging = false;
    }
    
    public Vector3 CastDirectionToVector(Direction direction) => direction switch
    {
        Direction.Down => Vector3.back,
        Direction.Up => Vector3.forward,
        Direction.Right => Vector3.right,
        Direction.Left => Vector3.left,
        _ => Vector3.zero
    };

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        
    }

    public void OnMouseLeftButton(InputAction.CallbackContext context)
    {
        
    }
}

public enum Direction
{
    None,
    Up,
    Down,
    Right,
    Left
}