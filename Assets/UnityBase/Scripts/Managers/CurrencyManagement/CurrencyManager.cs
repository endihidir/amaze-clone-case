using System;
using UnityBase.EventBus;
using UnityBase.Manager.Data;
using UnityBase.ManagerSO;
using UnityBase.Service;
using UnityEngine;

namespace UnityBase.Manager
{
    public class CurrencyManager : ICurrencyDataService, ICurrencyViewService, IAppPresenterDataService
    {
        private const string COIN_AMOUNT_KEY = "CoinAmountKey";
        public event Action<int> OnCoinDataUpdate;

        private EventBinding<GameStateData> _gameStateBinding = new EventBinding<GameStateData>();

        private ICoinView _coinView;

        #region VARIABLES

        private int _startCoinAmount;
        
        private bool _isCoinSaveAvailable;

        #endregion

        #region PROPERTIES

        public int SavedCoinAmount
        {
            get => GetCoin();
            private set => SetCoin(value);
        }

        #endregion

        public CurrencyManager(ManagerDataHolderSO managerDataHolderSo)
        {
            var currencyManagerData = managerDataHolderSo.currencyManagerSo;

            _startCoinAmount = currencyManagerData.startCoinAmount;
        }

        public void UpdateCoinView(ICoinView coinView)
        {
            _coinView = coinView;
        }

        ~CurrencyManager() { }

        public void Initialize() { }

        public void Start() { }

        public void Dispose() { }

        private int GetCoin() => PlayerPrefs.GetInt(COIN_AMOUNT_KEY, _startCoinAmount);
        private void SetCoin(int value) => PlayerPrefs.SetInt(COIN_AMOUNT_KEY, value);

        public void IncreaseCoinData(int value)
        {
            SavedCoinAmount += value;

            OnCoinDataUpdate?.Invoke(SavedCoinAmount);
        }

        public void DecreaseCoinData(int value)
        {
            SavedCoinAmount -= value;

            OnCoinDataUpdate?.Invoke(SavedCoinAmount);
        }

        public void UpdateCoinView(int value)
        {
            _coinView.UpdateView(value);
        }

        public void PlayViewAnimation()
        {
            _coinView.PlayCoinIconAnimation();
        }

        public Transform CoinIconTransform => _coinView.CoinIconT;
    }
}