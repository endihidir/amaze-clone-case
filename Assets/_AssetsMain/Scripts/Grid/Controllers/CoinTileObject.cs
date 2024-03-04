using System;
using UnityBase.Service;
using UnityEngine;
using VContainer;

public class CoinTileObject : TileObject, ICoinDrawer
{
    [Inject] 
    private readonly ICurrencyDataService _currencyDataService;

    [Inject] 
    private readonly ICurrencyViewService _currencyViewService;

    [SerializeField] private int _coinValue = 1;

    [SerializeField] private CoinAnimationHandler _coinAnimationHandler;
    
    private bool _isCollected;

    public bool IsCoinDisabled => !_coinAnimationHandler.IsActive;
    public Transform Transform => _coinAnimationHandler.transform;

    public override void Show(float duration, float delay, Action onComplete)
    {
        base.Show(duration, delay, onComplete);

        _coinAnimationHandler.StartIdleAnim();
    }

    public void CollectCoin(float movementStartDelay)
    {
        if(_isCollected) return;

        _isCollected = true;
        
        _currencyDataService.IncreaseCoinData(_coinValue);
        
        _coinAnimationHandler.StartMoveAnim(_currencyViewService.CoinIconTransform.position, movementStartDelay, OnCoinCollectComplete);
    }

    private void OnCoinCollectComplete()
    {
        _currencyViewService.UpdateCoinView(_coinValue);
        
        _currencyViewService.PlayViewAnimation();
    }
    
    public override void Reset()
    {
        base.Reset();

        _isCollected = false;
        
        _coinAnimationHandler.Reset();
    }
}