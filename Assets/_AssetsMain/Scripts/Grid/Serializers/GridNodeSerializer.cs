using System;
using UnityBase.Service;

public class GridNodeSerializer : IGridNodeSerializer
{
    private readonly IPoolDataService _poolDataService;
    public GridNodeSerializer(IPoolDataService poolDataService) => _poolDataService = poolDataService;

    public int Serialize(GridNode gridNode) => gridNode switch
    {
        PlayerTileObject => 2,
        TileObject => 0,
        BlockObject => 1,
        _ => 1
    };

    public T Deserialize<T>(int val) where T : GridNode => val switch
    {
        0 => _poolDataService.GetObject<TileObject>(0f,0f,default) as T,
        1 => _poolDataService.GetObject<BlockObject>(0f,0f, default) as T,
        2 => _poolDataService.GetObject<PlayerTileObject>(0f,0f, default) as T,
        _ => null
    };
    
    public Type Deserialize(int val) => val switch
    {
        0 => typeof(TileObject),
        1 => typeof(BlockObject),
        2 => typeof(PlayerTileObject),
        _ => null
    };
}