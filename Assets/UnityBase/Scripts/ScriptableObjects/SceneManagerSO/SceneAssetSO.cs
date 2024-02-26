using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UnityBase.ManagerSO
{
    [CreateAssetMenu(menuName = "Game/SceneManagement/SceneAsset")]
    public class SceneAssetSO : ScriptableObject
    {
        public AssetReference sceneReference;
    }
}