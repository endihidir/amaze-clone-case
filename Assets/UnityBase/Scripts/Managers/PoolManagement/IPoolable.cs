using System;
using UnityEngine;

namespace UnityBase.Pool
{
    public interface IPoolable
    {
        public Component PoolableObject { get; }
        public bool IsActive { get; }
        public bool IsUnique { get; }
        public void Show(float duration, float delay);
        public void Hide(float duration, float delay);
        public void OnHideComplete(Action act);
        public void InvokeHideComplete();
    }
}