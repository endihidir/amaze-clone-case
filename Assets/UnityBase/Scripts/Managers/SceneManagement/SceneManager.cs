using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityBase.Controller;
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
        public static Action<SceneType> OnSceneLoadComplete;

        private LoadingSceneController _loadingSceneController;

        private float _progressAnimationMultiplier = 10f;

        private SceneAssetSO _currentSceneAssetSo;

        private bool _sceneLoadInProgress;

        private SceneInstance _sceneInstance;

        private AsyncOperationHandle<SceneInstance> _asyncLoadOperationHandle, _asyncUnloadOperationHandle;

        private float _currentProgressValue, _targetProgressValue;

        private float _progressMultiplier = 0.1f;

        private bool _useLoadingScene;

        private CancellationTokenSource _cancellationTokenSource;

        private event Action<float> _onLoadUpdate;

        private SceneManagerSO _sceneManagerSo;

        public SceneManager(ManagerDataHolderSO managerDataHolderSo)
        {
            _sceneManagerSo = managerDataHolderSo.sceneManagerSo;

            _progressAnimationMultiplier = _sceneManagerSo.progressAnimationMultiplier;

            _loadingSceneController = new LoadingSceneController(_sceneManagerSo.GetSceneAsset(SceneType.Loading));
        }

        ~SceneManager()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        public void Initialize()
        {

        }

        public void Start()
        {
            
        }

        public void Dispose()
        {
            
        }

        public async void LoadSceneAsync(SceneType sceneType, bool useLoadingScene = false, float progressMultiplier = 1)
        {
            if (_sceneLoadInProgress)
            {
                return;
            }

            _useLoadingScene = useLoadingScene;

            _progressMultiplier = progressMultiplier;

            _currentSceneAssetSo = _sceneManagerSo.GetSceneAsset(sceneType);

            if (_asyncLoadOperationHandle.IsValid())
            {
                UnloadSceneAsync();
            }
            else
            {
                await LoadSceneAsync();
            }
        }

        private void UnloadSceneAsync()
        {
            _asyncUnloadOperationHandle = Addressables.UnloadSceneAsync(_sceneInstance);

            _asyncUnloadOperationHandle.Completed += OnUnloadSceneAsyncCompleted;
        }

        private async void OnUnloadSceneAsyncCompleted(AsyncOperationHandle<SceneInstance> asyncOperationHandle)
        {
            if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
            {
                _asyncUnloadOperationHandle.Completed -= OnUnloadSceneAsyncCompleted;

                await LoadSceneAsync();
            }
            else
            {
                Debug.Log("Failed to Unload!");
            }
        }

        private async UniTask LoadSceneAsync()
        {
            _sceneLoadInProgress = true;

            _currentProgressValue = 0;

            _targetProgressValue = 0;

            if (_useLoadingScene)
            {
                await _loadingSceneController.Initialize();
            }

            _asyncLoadOperationHandle = Addressables.LoadSceneAsync(_currentSceneAssetSo.sceneReference, LoadSceneMode.Additive, false);

            var delay = TimeSpan.FromSeconds(0.1f);

            await WaitProgress(delay);

            _asyncLoadOperationHandle.Completed += OnLoadSceneAsyncCompleted;
        }

        private async void OnLoadSceneAsyncCompleted(AsyncOperationHandle<SceneInstance> asyncOperationHandle)
        {
            if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
            {
                _asyncLoadOperationHandle.Completed -= OnLoadSceneAsyncCompleted;

                _sceneLoadInProgress = false;

                await _asyncLoadOperationHandle.Result.ActivateAsync();

                _sceneInstance = asyncOperationHandle.Result;

                if (_useLoadingScene)
                {
                    _loadingSceneController.ReleaseLoadingScene();
                }
            }
            else
            {
                Debug.Log("Failed to load!");
            }
        }

        private async UniTask WaitProgress(TimeSpan delay)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                do
                {
                    await UniTask.Delay(delay, DelayType.DeltaTime, PlayerLoopTiming.Update, _cancellationTokenSource.Token);

                    _targetProgressValue = _asyncLoadOperationHandle.PercentComplete / 0.9f;

                    var multiplier = _progressMultiplier * _progressAnimationMultiplier;

                    _currentProgressValue = Mathf.MoveTowards(_currentProgressValue, _targetProgressValue, multiplier * Time.deltaTime);

                    _onLoadUpdate?.Invoke(_currentProgressValue);

                } while (!Mathf.Approximately(_currentProgressValue, _targetProgressValue));
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public void OnLoadUpdate(Action<float> act)
        {
            _onLoadUpdate = act;
        }
    }
}