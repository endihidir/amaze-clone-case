﻿using System.Collections.Generic;
using UnityBase.Pool;
using UnityEngine;

namespace UnityBase.Service
{
    public interface IPoolDataService
    {
        public T GetObject<T>(float duration, float delay) where T : Component, IPoolable;
        public void HideObject<T>(T poolable, float duration, float delay, bool readLogs = false) where T : Component, IPoolable;
        public void HideAllObjectsOfType<T>(float duration, float delay, bool readLogs = false) where T : Component, IPoolable;
        public void HideAllTypeOf<T>(float duration, float delay) where T : Component, IPoolable;
        public void HideAll(float duration, float delay);
        public void Remove<T>(T poolable, bool readLogs = false) where T : Component, IPoolable;
        public void RemovePool<T>(bool readLogs = false) where T : Component, IPoolable;
        public int GetClonesCount<T>(bool readLogs = false) where T : Component, IPoolable;
        public List<T> GetClones<T>() where T : Component, IPoolable;
    }
}