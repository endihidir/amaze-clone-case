using Cysharp.Threading.Tasks;
using UnityBase.ManagerSO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace UnityBase.Controller
{
    public class LoadingSceneController
    {
        private SceneAssetSO _loadingScene;

        private AsyncOperationHandle<SceneInstance> _asyncOperationHandle;
        public LoadingSceneController(SceneAssetSO loadingScene) => _loadingScene = loadingScene;
        ~LoadingSceneController() { }
        public async UniTask InitializeAsync()
        {
            _asyncOperationHandle = _loadingScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive);

            await _asyncOperationHandle.Task;

            _asyncOperationHandle.Completed += OnLoadingSceneLoad;
        }

        private void OnLoadingSceneLoad(AsyncOperationHandle<SceneInstance> handle) => _asyncOperationHandle.Completed -= OnLoadingSceneLoad;
        public async UniTask ReleaseLoadingSceneAsync() => await Addressables.UnloadSceneAsync(_asyncOperationHandle.Result);
    }
}