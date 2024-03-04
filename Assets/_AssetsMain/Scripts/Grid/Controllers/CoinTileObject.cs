using System;
using UnityBase.Service;
using UnityEngine;
using VContainer;

public class CoinTileObject : TileObject, ICoinDrawer
{
    [Inject] 
    private readonly ICurrencyDataService _currencyDataService;
    
    [SerializeField] private CoinAnimationHandler _coinAnimationHandler;
    
    private bool _isCollected;
    
    private CoinUI _coinUI;

    public bool IsCoinDisabled => !_coinAnimationHandler.IsActive;
    public Transform Transform => _coinAnimationHandler.transform;
    public float StartDelay { get; set; }

    public override void Show(float duration, float delay, Action onComplete)
    {
        base.Show(duration, delay, onComplete);

        _coinUI ??= FindObjectOfType<CoinUI>();
        
        _coinAnimationHandler.StartIdleAnim();
    }

    public void CollectCoin()
    {
        if(_isCollected) return;

        _isCollected = true;
        
        _coinAnimationHandler.StartMoveAnim(_coinUI.CoinIconT.position, StartDelay, OnCoinCollectComplete);
    }

    private void OnCoinCollectComplete()
    {
        _coinUI.PlayCoinIconAnim();
        
        _currencyDataService.IncreaseCoin(1);
    }
    
    public override void Reset()
    {
        base.Reset();

        _isCollected = false;
        
        _coinAnimationHandler.Reset();
    }
}