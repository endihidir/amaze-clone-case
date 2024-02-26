using UnityEngine;

namespace UnityBase.ManagerSO
{
    [CreateAssetMenu(menuName = "Game/ManagerData/SceneManagerData")]
    public class SceneManagerSO : ScriptableObject
    {
        public float progressAnimationMultiplier = 10f;

        [SerializeField] private SceneAssetSO _loadingScene;
        [SerializeField] private SceneAssetSO _menuScene;
        [SerializeField] private SceneAssetSO _gameplayScene;

        public void Initialize()
        {

        }

        public SceneAssetSO GetSceneAsset(SceneType sceneType) => sceneType switch
        {
            SceneType.Loading => _loadingScene,
            SceneType.MainMenu => _menuScene,
            SceneType.Gameplay => _gameplayScene,
            _ => null
        };
    }
}

public enum SceneType
{
    Loading,
    MainMenu,
    Gameplay
}