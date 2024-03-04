using UnityEngine;

public interface ICoinDrawer 
{
    public bool IsCoinDisabled { get; }
    public Transform Transform { get; }
}
