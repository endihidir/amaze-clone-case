
public class PaintCompleteChecker
{
    public bool IsAllTilesPainted(Grid<TileBase> gridData)
    {
        for (int x = 0; x < gridData.Width; x++)
        {
            for (int z = 0; z < gridData.Height; z++)
            {
                var gridObject = gridData.GetGridObject(x, z);

                if (gridObject is TileObject { IsPainted: false }) return false;
            }
        }

        return true;
    }
}