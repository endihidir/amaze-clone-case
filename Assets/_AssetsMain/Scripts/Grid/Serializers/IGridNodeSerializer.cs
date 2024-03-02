using System;

public interface IGridNodeSerializer
{
    public int Serialize(TileBase tileBase);
    public int SerializeOnCoinCollect(TileBase tileBase);
    public T Deserialize<T>(int val) where T : TileBase;
    public Type Deserialize(int val);
}