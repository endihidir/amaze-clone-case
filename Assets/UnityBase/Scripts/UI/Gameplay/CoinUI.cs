using DG.Tweening;
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

    [SerializeField] private Transform _coinIconT;
    public Transform CoinIconT => _coinIconT;

    private void Awake() => UpdateView(_currencyDataService.SavedCoinAmount);

    private void OnEnable() => CurrencyManager.OnCoinDataUpdate += OnCoinDataUpdate;

    private void OnDisable() => CurrencyManager.OnCoinDataUpdate -= OnCoinDataUpdate;

    private void OnCoinDataUpdate(int coinVal) => UpdateView(coinVal);

    private void UpdateView(int val)
    {
        _coinTxt.text = val.ToString("0");
    }

    public void PlayCoinIconAnim()
    {
        _coinIconT.transform.DOKill(true);
        _coinIconT.transform.DOPunchScale(Vector3.one * 0.6f, 0.2f)
                            .OnComplete(()=> _coinIconT.transform.localScale = Vector3.one);
    }

    private void OnDestroy()
    {
        _coinIconT.transform.DOKill();
    }
}