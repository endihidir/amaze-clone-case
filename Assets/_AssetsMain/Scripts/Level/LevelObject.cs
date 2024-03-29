using System;
using DG.Tweening;
using Sirenix.Utilities;
using UnityBase.Pool;
using UnityBase.Service;
using UnityEngine;
using VContainer;

public class LevelObject : MonoBehaviour, IPoolable
{
    [Inject] 
    private readonly IPoolDataService _poolDataService;
    
    [SerializeField] private Transform _gridsParent, _ballsParent;

    private float _endXPos;
    private Tween _moveTween;
    private IActivatable[] _ballInputs;
    public Transform GridsParent => _gridsParent;
    public Transform BallsParent => _ballsParent;
    
    public Component PoolableObject => this;
    public bool IsActive => isActiveAndEnabled;
    public bool IsUnique => false;
    
    public void Show(float duration, float delay, Action onComplete)
    {
        gameObject.SetActive(true);
        
        _moveTween?.Kill();

        _moveTween = transform.DOMoveX(0f, duration)
                              .SetDelay(delay)
                              .OnComplete(()=> OnShowComplete(onComplete))
                              .SetEase(Ease.InOutQuad);
    }

    public void Hide(float duration, float delay, Action onComplete)
    {
        _moveTween?.Kill();

        _moveTween = transform.DOMoveX(_endXPos, duration)
                              .SetDelay(delay)
                              .OnComplete(() => OnHideComplete(onComplete))
                              .SetEase(Ease.InOutQuad);
    }

    private void OnShowComplete(Action onComplete)
    {
        ActivateInput();
        onComplete?.Invoke();
    }
    private void OnHideComplete(Action onComplete)
    {
        onComplete?.Invoke();
        
        var allTileResettables = GetComponentsInChildren<IResettable>();
        allTileResettables.ForEach(x => x.Reset());
        
        var allPoolables = GetComponentsInChildren<IPoolable>();
        allPoolables.ForEach(x => _poolDataService.HideObject(x, 0f,0f));
        
        gameObject.SetActive(false);
    }

    public void ActivateInput()
    {
        _ballInputs = GetComponentsInChildren<IActivatable>();
        _ballInputs.ForEach(x => x.ActivateInitials(true));
    }
    
    public void DeactivateInput()
    {
        _ballInputs.ForEach(x => x.ActivateInitials(true));
    }
    public void SetEndXPos(float endXPos)
    {
        _endXPos = endXPos;
    }
    
    private void OnDestroy()
    {
        _moveTween?.Kill();
        _poolDataService.Remove(this);
    }
}