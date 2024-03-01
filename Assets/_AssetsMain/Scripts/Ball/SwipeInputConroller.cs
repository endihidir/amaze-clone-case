using UnityBase.Service;
using UnityEngine;

public class SwipeInputConroller : ISwipeInputService, IGameplayPresenterDataService
{
    private bool _detectSwipeOnlyAfterRelease = false;
    private float _minDistanceForSwipe = 90f;
    
    private Vector2 _fingerDownPosition;
    private Vector2 _fingerUpPosition;

    private Direction _direction;

    private bool _isDragging;
    public void Initialize() { }
    public void Start() { }
    public void Dispose() { }
    
    public Direction GetSwipeDirection()
    {
        foreach (var touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                _fingerDownPosition = touch.position;
                _fingerUpPosition = touch.position;
            }

            if (!_detectSwipeOnlyAfterRelease && touch.phase == TouchPhase.Moved)
            {
                _fingerUpPosition = touch.position;
                CheckSwipe();
            }

            if (touch.phase == TouchPhase.Ended)
            {
                _fingerUpPosition = touch.position;
                CheckSwipe();
            }
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            _isDragging = true;
            _fingerDownPosition = Input.mousePosition;
            _fingerUpPosition = Input.mousePosition;
        }

        if (_isDragging)
        {
            _fingerUpPosition = Input.mousePosition;
            CheckSwipe();
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            _fingerUpPosition = Input.mousePosition;
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

    public Vector3 CastDirectionToVector(Direction direction) => direction switch
    {
        Direction.Down => Vector3.back,
        Direction.Up => Vector3.forward,
        Direction.Right => Vector3.right,
        Direction.Left => Vector3.left,
        _ => Vector3.zero
    };
}

public enum Direction
{
    None,
    Up,
    Down,
    Right,
    Left
}