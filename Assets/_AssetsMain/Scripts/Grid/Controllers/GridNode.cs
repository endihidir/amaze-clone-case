using System;
using Sirenix.OdinInspector;
using UnityBase.Pool;
using UnityBase.Service;
using UnityEngine;
using VContainer;

public abstract class GridNode : MonoBehaviour, IPoolable
{
    [Required] public Texture2D icon;
    
    [Inject] 
    private readonly IPoolDataService _poolDataService;
    
    public int index;
    public Component PoolableObject => this;
    public virtual bool IsActive => isActiveAndEnabled;
    public virtual bool IsUnique => false;
    
    private event Action _onHideComplete;

    public virtual void Show(float duration, float delay) => gameObject.SetActive(true);
    public virtual void Hide(float duration, float delay)
    {
        gameObject.SetActive(false);
        InvokeHideComplete();
    }

    public void OnHideComplete(Action act) => _onHideComplete = act;
    public void InvokeHideComplete() => _onHideComplete?.Invoke();
    protected virtual void OnDestroy() => _poolDataService.Remove(this);
}