using System;
using UnityBase.Service;

public class GridNodeSerializer : IGridNodeSerializer
{
    private readonly IPoolDataService _poolDataService;
    public GridNodeSerializer(IPoolDataService poolDataService) => _poolDataService = poolDataService;

    public int Serialize(TileBase tileBase) => tileBase switch
    {
        CoinTileObject => 3,
        PlayerTileObject => 2,
        TileObject => 0,
        BlockTileObject => 1,
        _ => 1
    };
    
    public int SerializeOnCoinCollect(TileBase tileBase) => tileBase switch
    {
        PlayerTileObject => 2,
        TileObject => 0,
        BlockTileObject => 1,
        _ => 1
    };

    public T Deserialize<T>(int val) where T : TileBase => val switch
    {
        0 => _poolDataService.GetObject<TileObject>(0f,0f,default) as T,
        1 => _poolDataService.GetObject<BlockTileObject>(0f,0f, default) as T,
        2 => _poolDataService.GetObject<PlayerTileObject>(0f,0f, default) as T,
        3 => _poolDataService.GetObject<CoinTileObject>(0f, 0f) as T,
        _ => null
    };
    
    
    public Type Deserialize(int val) => val switch
    {
        0 => typeof(TileObject),
        1 => typeof(BlockTileObject),
        2 => typeof(PlayerTileObject),
        3 => typeof(CoinTileObject),
        _ => null
    };
}