using System;
using DG.Tweening;
using UnityBase.Pool;
using UnityBase.Service;
using UnityEngine;
using VContainer;

public class BulletTest : MonoBehaviour, IPoolable
{
    private Tween _scaleTween;
    public Component PoolableObject => this;
    public bool IsActive => isActiveAndEnabled;
    public bool IsUnique => false;
    
    protected Collider _collider;

    [Inject]
    protected readonly IPoolDataService _poolDataService;
    
    private event Action _onHideComplete;
    
    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public void Show(float duration, float delay)
    {
        transform.localScale = Vector3.one;
        
        gameObject.SetActive(true);

        _collider.enabled = true;
    }
    
    public void Hide(float duration, float delay)
    {
        _scaleTween.Kill();
        
        _scaleTween = transform.DOScale(0f, duration).OnComplete(Disable);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
        
        InvokeHideComplete();
    }

    private void Update()
    {
        if(_scaleTween.IsActive()) return;
        
        transform.position += transform.forward * (Time.deltaTime * 10f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Tag_WallTest _))
        {
            _collider.enabled = false;
            
            _poolDataService.HideObject(this, 1f, 0f);
        }
    }
    
    public void OnHideComplete(Action act) => _onHideComplete = act;
    public void InvokeHideComplete() => _onHideComplete?.Invoke();

    private void OnDestroy()
    {
        _scaleTween.Kill();
        
        _poolDataService.Remove(this);
    }
}