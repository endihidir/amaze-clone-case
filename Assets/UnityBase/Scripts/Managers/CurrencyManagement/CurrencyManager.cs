using System;
using UnityBase.EventBus;
using UnityBase.Manager.Data;
using UnityBase.ManagerSO;
using UnityBase.Service;
using UnityEngine;

namespace UnityBase.Manager
{
    public class CurrencyManager : ICurrencyDataService, IAppPresenterDataService
    {
        private const string COIN_AMOUNT_KEY = "CoinAmountKey";

        public static event Action<float> OnCoinUpdate;

        private EventBinding<GameStateData> _gameStateBinding = new EventBinding<GameStateData>();

        #region VARIABLES

        private float _startCoinAmount;
        private bool _isCoinSaveAvailable;

        private float _currentManaAmount;
        private float _coinAmountHolder;
        private float _unsavedCoinAmount;

        #endregion

        #region PROPERTIES

        public float SavedCoinAmount
        {
            get => GetCoin();
            private set => SetCoin(value);
        }

        #endregion

        public CurrencyManager(ManagerDataHolderSO managerDataHolderSo)
        {
            var currencyManagerData = managerDataHolderSo.currencyManagerSo;

            _startCoinAmount = currencyManagerData.startCoinAmount;
            _isCoinSaveAvailable = currencyManagerData.isCoinSaveAvailable;

            _unsavedCoinAmount = _startCoinAmount;
            _coinAmountHolder = SavedCoinAmount;
        }

        ~CurrencyManager()
        {
            Dispose();
        }

        public void Initialize()
        {

        }

        public void Start()
        {
            _gameStateBinding.Add(OnCompleteGameStateTransition);

            EventBus<GameStateData>.AddListener(_gameStateBinding, GameStateData.GetChannel(TransitionState.End));
        }

        public void Dispose()
        {
            _gameStateBinding.Remove(OnCompleteGameStateTransition);
            
            EventBus<GameStateData>.RemoveListener(_gameStateBinding, GameStateData.GetChannel(TransitionState.End));
        }

        private void OnCompleteGameStateTransition(GameStateData gameStateData)
        {
            if (gameStateData.EndState is GameState.GameLoadingState or GameState.GameFailState)
            {
                SaveCoinAmount();
            }
        }

        private float GetCoin()
        {
            var prefsVal = PlayerPrefs.GetFloat(COIN_AMOUNT_KEY, _startCoinAmount);
            var val = _isCoinSaveAvailable ? prefsVal : _unsavedCoinAmount;
            return val;
        }

        private void SetCoin(float value)
        {
            if (_isCoinSaveAvailable)
                PlayerPrefs.SetFloat(COIN_AMOUNT_KEY, value);
            else
                _unsavedCoinAmount = value;
        }

        public void IncreaseCoin(float value)
        {
            _coinAmountHolder += value;

            OnCoinUpdate?.Invoke(_coinAmountHolder);
        }

        public void DecreaseCoin(float value)
        {
            _coinAmountHolder -= value;

            OnCoinUpdate?.Invoke(_coinAmountHolder);
        }

        private void SaveCoinAmount()
        {
            SavedCoinAmount = _coinAmountHolder;
        }
    }
}