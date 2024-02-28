using System;

public interface IGridNodeSerializer
{
    public int Serialize(GridNode gridNode);
    public T Deserialize<T>(int val) where T : GridNode;
    public Type Deserialize(int val);
}