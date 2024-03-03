using TMPro;
using UnityBase.Manager;
using UnityBase.Service;
using UnityEngine;
using VContainer;

public class CoinUI : MonoBehaviour
{
    [Inject] 
    private readonly ICurrencyDataService _currencyDataService;
    
    [SerializeField] private TextMeshProUGUI _coinTxt;

    private void Awake() => UpdateView(_currencyDataService.SavedCoinAmount);

    private void OnEnable() => CurrencyManager.OnCoinDataUpdate += OnCoinDataUpdate;

    private void OnDisable() => CurrencyManager.OnCoinDataUpdate -= OnCoinDataUpdate;

    private void OnCoinDataUpdate(int coinVal) => UpdateView(coinVal);

    private void UpdateView(int val) => _coinTxt.text = val.ToString("0");
}