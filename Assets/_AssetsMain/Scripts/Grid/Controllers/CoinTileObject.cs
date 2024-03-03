using DG.Tweening;
using UnityBase.Manager;
using UnityEngine;

public class CoinTileObject : TileObject, ICollectibleDrawer
{
    [SerializeField] private GameObject _coinObject;
    
    private bool _isCollected;
    
    private float _defaultZPos;
    
    private Tween _bounceTween;
    public bool IsCollected => _isCollected;
    public Transform Transform => _coinObject.transform;
    
    private void Awake()
    {
        var delay = Random.Range(0f, 0.5f);

        _defaultZPos = _coinObject.transform.localPosition.z;

        _bounceTween = DOTween.Sequence()
                              .AppendInterval(0.5f)
                              .Append(_coinObject.transform.DOLocalMoveZ(_defaultZPos + 0.1f, 0.2f).SetEase(Ease.OutCubic).SetDelay(delay))
                              .SetLoops(-1, LoopType.Yoyo);
    }

    public void CollectCoin()
    {
        if(_isCollected) return;

        _isCollected = true;
        
        _coinObject.SetActive(false);
        
        _bounceTween.Kill();
        
        CurrencyManager.OnCoinCollect?.Invoke(_coinObject.transform.position, 1);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _bounceTween.Kill();
    }
}