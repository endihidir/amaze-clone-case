using UnityBase.Manager;
using UnityBase.Service;
using UnityBase.UI.ButtonCore;
using VContainer;

public class NextLevelButton : ButtonBehaviour
{
    [Inject]
    private readonly IGameDataService _gameDataService;

    [Inject] 
    private readonly IGameplayDataService _gameplayDataService;

    protected override void OnClick()
    {
        _gameDataService.PlayGame();

        _gameplayDataService.ChangeGameState(GameState.GamePlayState, 0f);
    }
}