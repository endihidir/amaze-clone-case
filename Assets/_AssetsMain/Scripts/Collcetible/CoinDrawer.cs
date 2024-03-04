using System.Collections.Generic;
using System.Linq;
using UnityBase.EventBus;
using UnityBase.Manager;
using UnityBase.Manager.Data;
using UnityBase.Service;
using UnityEngine;
using VContainer;

public class CoinDrawer : MonoBehaviour
{
    [Inject] 
    private readonly ILevelDataService _levelDataService;
    
    [SerializeField] private Mesh _collectibleMesh;
    
    [SerializeField] private Material _collectibleMaterial;
    
    private List<ICoinDrawer> _collectibles;

    private List<Matrix4x4> _matrices = new List<Matrix4x4>();

    private ICoinDrawer[] _collectibleDatas;

    private EventBinding<GameStateData> _gameStateBinding = new EventBinding<GameStateData>();

    private bool _isInitialized;

    private void OnEnable()
    {
        _gameStateBinding.Add(OnGameStateTransition);
        EventBus<GameStateData>.AddListener(_gameStateBinding, GameStateData.GetChannel(TransitionState.Start));
    }
    
    private void OnDisable()
    {
        _gameStateBinding.Remove(OnGameStateTransition);
        EventBus<GameStateData>.RemoveListener(_gameStateBinding, GameStateData.GetChannel(TransitionState.Start));
        
    }
    private void OnGameStateTransition(GameStateData gameStateData)
    {
        if (!_levelDataService.GetCurrentLevelData().hasUpdateableData)
        {
            _isInitialized = false;
            return;
        }
        
        var isGameStarted = gameStateData.StartState == GameState.GameLoadingState && gameStateData.EndState == GameState.GamePlayState;
        
        var passedToNextLevel = gameStateData.StartState == GameState.GameSuccessState && gameStateData.EndState == GameState.GamePlayState;
        
        if (isGameStarted || passedToNextLevel)
        {
            Initialize();
        }
    }

    private void Initialize()
    {
        _collectibles = FindObjectsOfType<MonoBehaviour>().OfType<ICoinDrawer>().ToList();

        for (int i = 0; i < _collectibles.Count; i++)
        {
            var collectibleT = _collectibles[i].Transform;
            
            _matrices.Add(Matrix4x4.TRS(collectibleT.position, collectibleT.rotation, collectibleT.localScale));
        }

        _isInitialized = _collectibles.Count > 1;
    }

    private void Update()
    {
        if(!_isInitialized) return;
        
        for (int i = 0; i < _collectibles.Count; i++)
        {
            var collectibleT = _collectibles[i].Transform;
            
            _matrices[i] = Matrix4x4.TRS(collectibleT.position, collectibleT.rotation, collectibleT.localScale);
            
            if (_collectibles[i].IsCoinDisabled)
            {
                _collectibles.RemoveAt(i);
                _matrices.RemoveAt(i);
            }
        }
        
        Graphics.DrawMeshInstanced(_collectibleMesh, 0, _collectibleMaterial, _matrices);
    }
}