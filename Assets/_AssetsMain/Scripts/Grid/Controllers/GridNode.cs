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
    protected readonly IPoolDataService _poolDataService;

    public int index;
    public Component PoolableObject => this;
    public virtual bool IsActive => isActiveAndEnabled;
    public virtual bool IsUnique => false;

    public virtual void Show(float duration, float delay, Action onComplete) => gameObject.SetActive(true);
    public virtual void Hide(float duration, float delay, Action onComplete)
    {
        gameObject.SetActive(false);
    }
    
    protected virtual void OnDestroy() => _poolDataService.Remove(this);
}