using System;
using DG.Tweening;
using UnityBase.Pool;
using UnityBase.Service;
using UnityEngine;
using VContainer;

public class CoinIconUI : MonoBehaviour, IPoolable
{
    [Inject] 
    private readonly IPoolDataService _poolDataService;
    public Component PoolableObject => this;
    public bool IsActive => isActiveAndEnabled;
    public bool IsUnique => false;

    private Tween _moveTween;
    public void Show(float duration, float delay, Action onComplete) => gameObject.SetActive(true);
    public void Hide(float duration, float delay, Action onComplete)
    {
        gameObject.SetActive(false);
        transform.localScale = Vector3.one * 0.5f;
    }

    public void MoveTo(Transform target, float delay, Action onComplete)
    {
        _moveTween = DOTween.Sequence()
                            .AppendInterval(delay)
                            .Append(transform.DOMove(target.position, 0.6f).SetEase(Ease.InBack))
                            .Join(transform.DOScale(0.75f, 0.6f).SetEase(Ease.OutBack))
                            .Append(transform.DOScale(1.5f, 0.15f).SetEase(Ease.InQuad))
                            .AppendCallback(() => onComplete?.Invoke());
    }
    
    private void OnDestroy()
    {
        _moveTween.Kill();
        _poolDataService.Remove(this);
    }
}