using TMPro;
using UnityBase.EventBus;
using UnityBase.Manager;
using UnityBase.Manager.Data;
using UnityBase.Service;
using UnityEngine;
using VContainer;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelTxt;
    
    [Inject] 
    private readonly ILevelDataService _levelDataService;

    private EventBinding<GameStateData> _gameStateBinding = new();

    private void Awake()
    {
        _levelTxt.text = "LEVEL " + _levelDataService.LevelText.ToString("0");
    }

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
        if (gameStateData.StartState == GameState.GameSuccessState && gameStateData.EndState == GameState.GamePlayState)
        {
            _levelTxt.text = "LEVEL " + _levelDataService.LevelText.ToString("0");      
        }
    }
}
