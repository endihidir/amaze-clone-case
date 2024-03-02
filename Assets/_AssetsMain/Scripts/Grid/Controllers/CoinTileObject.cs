using UnityEngine;

public class CoinTileObject : TileObject
{
    [SerializeField] private GameObject _coinObject;

    private bool _isCollected;
    
    public void CollectCoin()
    {
        if(_isCollected) return;

        _isCollected = true;
        
        _coinObject.SetActive(false);
    }
}