using UnityEngine;

public interface ISwipeInputService
{
    public Direction GetSwipeDirection();
    public Vector3 CastDirectionToVector(Direction direction);
    public void ResetInput();
}