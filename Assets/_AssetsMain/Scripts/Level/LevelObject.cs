using System;
using DG.Tweening;
using UnityBase.Pool;
using UnityBase.Service;
using UnityEngine;
using VContainer;

public class LevelObject : MonoBehaviour, IPoolable
{
    [Inject] 
    private readonly IPoolDataService _poolDataService;
    
    [SerializeField] private Transform _gridsParent, _ballsParent;

    public Component PoolableObject => this;
    public bool IsActive => isActiveAndEnabled;
    public bool IsUnique => false;
    
    private float _endXPos;
    
    private Tween _moveTween;
    public Transform GridsParent => _gridsParent;

    public Transform BallsParent => _ballsParent;

    public void SetEndPos(float endXPos) => _endXPos = endXPos;
    public void Show(float duration, float delay, Action onComplete)
    {
        gameObject.SetActive(true);
        
        _moveTween?.Kill();

        _moveTween = transform.DOMoveX(0f, duration)
                              .SetDelay(delay)
                              .OnComplete(()=> OnShowComplete(onComplete))
                              .SetEase(Ease.InOutQuad);
    }

    private void OnShowComplete(Action onComplete)
    {
        ActivateInput();
        onComplete?.Invoke();
    }

    public void Hide(float duration, float delay, Action onComplete)
    {
        DeactivateInput();
        
        _moveTween?.Kill();

        _moveTween = transform.DOMoveX(_endXPos, duration)
                              .SetDelay(delay)
                              .OnComplete(() => OnHideComplete(onComplete))
                              .SetEase(Ease.InOutQuad);
    }

    private void OnHideComplete(Action onComplete)
    {
        onComplete?.Invoke();
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _moveTween?.Kill();
        _poolDataService.Remove(this);
    }

    private void ActivateInput()
    {
        
    }

    private void DeactivateInput()
    {
        
    }
}