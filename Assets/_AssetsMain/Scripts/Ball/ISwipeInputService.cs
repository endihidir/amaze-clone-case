using UnityEngine;

public interface ISwipeInputService
{
    public SwipeDirection GetSwipeDirection();

    public Vector3 CastDirectionToVector(SwipeDirection swipeDirection);
}