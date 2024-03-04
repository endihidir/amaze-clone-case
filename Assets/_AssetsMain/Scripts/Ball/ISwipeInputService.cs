using UnityEngine;

public interface ISwipeInputService
{
    public Direction GetSwipeDirection();
    public Vector3 SerializeDirection(Direction direction);
    public void ResetInput();
}