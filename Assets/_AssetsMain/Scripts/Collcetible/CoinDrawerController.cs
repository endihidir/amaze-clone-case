using UnityBase.EventBus;
using UnityBase.Manager;
using UnityBase.Manager.Data;
using UnityBase.Service;
using UnityEngine;
using VContainer;

public class CoinDrawerController : MonoBehaviour
{
    [Inject] 
    private readonly ILevelDataService _levelDataService;

    [SerializeField] private CoinDrawer _coinDrawer;

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
            _coinDrawer.Disable();
            return;
        }
        
        var isGameStarted = gameStateData.StartState == GameState.GameLoadingState && gameStateData.EndState == GameState.GamePlayState;
        
        var passedToNextLevel = gameStateData.StartState == GameState.GameSuccessState && gameStateData.EndState == GameState.GamePlayState;
        
        if (isGameStarted || passedToNextLevel)
        {
            _coinDrawer.Initialize();
        }
        else
        {
            _coinDrawer.Disable();
        }
    }
}
