using UnityEngine;

namespace UnityBase.ManagerSO
{
    [CreateAssetMenu(menuName = "Game/ManagerData/CurrencyManagerData")]
    public class CurrencyManagerSO : ScriptableObject
    {
        public float startCoinAmount = 0f;

        public bool isCoinSaveAvailable = true;

        public void Initialize()
        {

        }
    }
}