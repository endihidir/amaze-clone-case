using System;

namespace UnityBase.Service
{
    public interface ISceneDataService
    {
        public void LoadSceneAsync(SceneType sceneType, bool useLoadingScene = false, float progressMultiplier = 1);
        public void OnLoadUpdate(Action<float> act);
    }
}