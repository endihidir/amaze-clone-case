using Unity.Mathematics;

public class GridGenerator : IGridEntity
{
    public int Width { get; }
    public int Height { get; }
    public float NodeSize { get; }
    public float3 Padding { get; }
    public float3 OriginPos { get; }

    public GridGenerator(int width, int height, float nodeSize, float3 padding, float3 originPos)
    {
        Width = width;
        Height = height;
        NodeSize = nodeSize;
        Padding = padding;
        OriginPos = originPos;
    }

    public void GenerateGrid()
    {
        
    }
}