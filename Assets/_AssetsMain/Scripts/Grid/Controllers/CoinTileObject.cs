using System;
using DG.Tweening;
using UnityBase.Service;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

public class CoinTileObject : TileObject, ICollectibleDrawer
{
    [Inject] 
    private readonly ICurrencyDataService _currencyDataService;
    
    [SerializeField] private GameObject _coinObject;
    
    private bool _isCollected;
    
    private float _defaultZPos;
    
    private Tween _bounceTween;
    public bool IsCollected => _isCollected || !isActiveAndEnabled;
    public Transform Transform => _coinObject.transform;

    public override void Show(float duration, float delay, Action onComplete)
    {
        base.Show(duration, delay, onComplete);
        
        var startDelay = Random.Range(0f, 0.5f);

        _defaultZPos = _coinObject.transform.localPosition.z;

        _bounceTween = DOTween.Sequence()
                                .AppendInterval(0.5f)
                                .Append(_coinObject.transform.DOLocalMoveZ(_defaultZPos + 0.1f, 0.2f).SetEase(Ease.OutCubic).SetDelay(startDelay))
                                .SetLoops(-1, LoopType.Yoyo);
    }

    public void CollectCoin()
    {
        if(_isCollected) return;

        _isCollected = true;
        
        _coinObject.SetActive(false);
        
        _bounceTween?.Kill();
        
        _currencyDataService.IncreaseCoin(1);
        
        //CurrencyManager.OnCoinCollect?.Invoke(_coinObject.transform.position, 1);
    }

    public override void Reset()
    {
        base.Reset();

        _isCollected = false;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _bounceTween.Kill();
    }
}