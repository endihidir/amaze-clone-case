using UnityEngine;

namespace UnityBase.Service
{
    public interface ICurrencyViewService
    {
        public void UpdateCoinView(int value);
        public void PlayViewAnimation();
        public Transform CoinIconTransform { get; }
    }
}