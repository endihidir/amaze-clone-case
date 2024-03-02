using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityBase.Controller;
using UnityBase.Extensions;
using UnityBase.ManagerSO;
using UnityBase.Service;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace UnityBase.Manager
{
    public class SceneManager : ISceneDataService, IAppPresenterDataService
    {
        private readonly SceneManagerSO _sceneManagerSo;
        
        public static Action<SceneType> OnSceneLoadComplete;

        private SceneAssetSO _currentSceneAssetSo;
        
        private LoadingSceneController _loadingSceneController;

        private bool _sceneLoadInProgress, _useLoadingScene;

        private AsyncOperationHandle<SceneInstance> _asyncLoadOperationHandle;

        private float _currentProgressValue, _targetProgressValue, _progressMultiplier = 0.1f, _progressAnimationMultiplier = 10f;

        private CancellationTokenSource _cancellationTokenSource;
        private event Action<float> _onLoadUpdate;
        public SceneManager(ManagerDataHolderSO managerDataHolderSo) => _sceneManagerSo = managerDataHolderSo.sceneManagerSo;
        
        ~SceneManager() => Dispose();

        public void Initialize()
        {
            _progressAnimationMultiplier = _sceneManagerSo.progressAnimationMultiplier;

            _loadingSceneController = new LoadingSceneController(_sceneManagerSo.GetSceneAsset(SceneType.Loading));
        }

        public void Start() { }
        
        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            _asyncLoadOperationHandle = default;
            _currentSceneAssetSo = null;
            _loadingSceneController = null;
        }

        public async void LoadSceneAsync(SceneType sceneType, bool useLoadingScene = false, float progressMultiplier = 1)
        {
            if (_sceneLoadInProgress) return;

            _useLoadingScene = useLoadingScene;

            _progressMultiplier = progressMultiplier;

            _currentSceneAssetSo = _sceneManagerSo.GetSceneAsset(sceneType);

            if (_asyncLoadOperationHandle.IsValid())
            {
                var isSucceed = await UnloadSceneAsync();

                if (isSucceed)
                {
                    await LoadSceneAsync();
                }
            }
            else
            {
                await LoadSceneAsync();
            }
        }

        private async UniTask<bool> UnloadSceneAsync()
        {
            var asyncUnloadOperationHandle = Addressables.UnloadSceneAsync(_asyncLoadOperationHandle.Result);

            await asyncUnloadOperationHandle.Task;

            return !asyncUnloadOperationHandle.IsValid();
        }

        private async UniTask LoadSceneAsync()
        {
            _sceneLoadInProgress = true;

            _currentProgressValue = 0;

            _targetProgressValue = 0;

            try
            {
                if (_useLoadingScene)
                {
                    await _loadingSceneController.InitializeAsync();
                }

                _asyncLoadOperationHandle = Addressables.LoadSceneAsync(_currentSceneAssetSo.sceneReference,
                    LoadSceneMode.Additive, false);

                await WaitProgressAsync(0.1f);

                if (_asyncLoadOperationHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    await _asyncLoadOperationHandle.Result.ActivateAsync();

                    if (_useLoadingScene)
                    {
                        await _loadingSceneController.ReleaseLoadingSceneAsync();
                    }

                    _sceneLoadInProgress = false;
                }
            } catch (Exception e) { Debug.Log(e); }
        }

        private async UniTask WaitProgressAsync(float delay)
        {
            CancellationTokenExtentions.Refresh(ref _cancellationTokenSource);
            
            do
            {
                await UniTask.WaitForSeconds(delay, false, PlayerLoopTiming.Update, _cancellationTokenSource.Token);
                _targetProgressValue = _asyncLoadOperationHandle.PercentComplete / 0.9f;
                var multiplier = _progressMultiplier * _progressAnimationMultiplier;
                _currentProgressValue = Mathf.MoveTowards(_currentProgressValue, _targetProgressValue, multiplier * Time.deltaTime);
                _onLoadUpdate?.Invoke(_currentProgressValue);

            } while (!Mathf.Approximately(_currentProgressValue, _targetProgressValue));
        }

        public void OnLoadUpdate(Action<float> act) => _onLoadUpdate = act;
    }
}