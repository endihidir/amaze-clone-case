using Unity.Mathematics;

public interface IGridEntity
{
    public int Width { get; }
    public int Height { get; }
    public float NodeSize { get; }
    public float3 Padding { get; }
    public float3 OriginPos { get; }
}