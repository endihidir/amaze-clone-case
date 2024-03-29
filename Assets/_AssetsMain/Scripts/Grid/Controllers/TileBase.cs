using System;
using Sirenix.OdinInspector;
using UnityBase.Pool;
using UnityBase.Service;
using UnityEngine;
using VContainer;

public abstract class TileBase : MonoBehaviour, IPoolable
{
    [ReadOnly] public int index;
    
    [Required] public Texture2D icon;
    
    [Inject] 
    protected readonly IPoolDataService _poolDataService;

    public Component PoolableObject => this;
    public virtual bool IsActive => isActiveAndEnabled;
    public virtual bool IsUnique => false;

    public virtual void Show(float duration, float delay, Action onComplete)
    {
        gameObject.SetActive(true);
        onComplete?.Invoke();
    }

    public virtual void Hide(float duration, float delay, Action onComplete)
    {
        gameObject.SetActive(false);
        onComplete?.Invoke();
    }

    protected virtual void OnDestroy() => _poolDataService.Remove(this);
}