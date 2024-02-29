using UnityBase.Service;
using UnityEngine;

public class SwipeInputConroller : ISwipeInputService, IGameplayPresenterDataService
{
    private bool _detectSwipeOnlyAfterRelease = false;
    private float _minDistanceForSwipe = 60f;
    
    private Vector2 _fingerDownPosition;
    private Vector2 _fingerUpPosition;

    private SwipeDirection _swipeDirection;

    private bool _isDragging;
    public void Initialize() { }
    public void Start() { }
    public void Dispose() { }
    
    public SwipeDirection GetSwipeDirection()
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

        return _swipeDirection;
    }

    private void CheckSwipe()
    {
        var deltaX = _fingerUpPosition.x - _fingerDownPosition.x;
        var deltaY = _fingerUpPosition.y - _fingerDownPosition.y;

        if (!(Mathf.Abs(deltaX) > _minDistanceForSwipe) && !(Mathf.Abs(deltaY) > _minDistanceForSwipe))
        {
            _swipeDirection = SwipeDirection.None;
            return;
        }
        
        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
        {
            _swipeDirection = deltaX > 0 ? SwipeDirection.Right : deltaX < 0 ? SwipeDirection.Left : SwipeDirection.None;
        }
        else
        {
            _swipeDirection = deltaY > 0 ? SwipeDirection.Up : deltaY < 0 ? SwipeDirection.Down : SwipeDirection.None;
        }

        _fingerDownPosition = _fingerUpPosition;
    }

    public Vector3 CastDirectionToVector(SwipeDirection swipeDirection) => swipeDirection switch
    {
        SwipeDirection.Down => Vector3.back,
        SwipeDirection.Up => Vector3.forward,
        SwipeDirection.Right => Vector3.right,
        SwipeDirection.Left => Vector3.left,
        _ => Vector3.zero
    };
}

public enum SwipeDirection
{
    None,
    Up,
    Down,
    Right,
    Left
}