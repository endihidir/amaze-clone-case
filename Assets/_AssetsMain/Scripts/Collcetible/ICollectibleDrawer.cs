using UnityEngine;

public interface ICollectibleDrawer 
{
    public bool IsCollected { get; }
    public Transform Transform { get; }
}
