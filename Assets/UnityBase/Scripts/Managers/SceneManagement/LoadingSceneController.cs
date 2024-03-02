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

        private SceneInstance _sceneInstance;

        public LoadingSceneController(SceneAssetSO loadingScene)
        {
            _loadingScene = loadingScene;
        }

        ~LoadingSceneController()
        {

        }

        public async UniTask Initialize()
        {
            _asyncOperationHandle = _loadingScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive);

            await _asyncOperationHandle.Task;

            _asyncOperationHandle.Completed += OnLoadingSceneLoad;
        }

        private void OnLoadingSceneLoad(AsyncOperationHandle<SceneInstance> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _sceneInstance = handle.Result;
            }
        }

        public void ReleaseLoadingScene()
        {
            Addressables.UnloadSceneAsync(_sceneInstance);

            _asyncOperationHandle.Completed -= OnLoadingSceneLoad;
        }
    }
}