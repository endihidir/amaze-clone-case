using UnityBase.Manager;
using UnityBase.Service;
using UnityEngine;
using VContainer;

public class CoinAnimationController : MonoBehaviour
{
    [Inject] 
    private readonly IPoolDataService _poolDataService;

    [Inject] 
    private readonly ICurrencyDataService _currencyDataService;

    [SerializeField] private Transform _coinIcon;

    private Camera _cam;

    private float _delay;
    private void Awake() => _cam = Camera.main;

    private void OnEnable() => CurrencyManager.OnCoinCollect += OnCoinCollect;

    private void OnDisable() => CurrencyManager.OnCoinCollect -= OnCoinCollect;

    private void OnCoinCollect(Vector3 coinWorldPosition, int value)
    {
        var uiStartPos = _cam.WorldToScreenPoint(coinWorldPosition);

        var coinUI = _poolDataService.GetObject<CoinIconUI>(0f, 0f);
        coinUI.transform.SetParent(transform);
        coinUI.transform.position = uiStartPos;

        coinUI.MoveTo(_coinIcon, _delay, ()=> UpdateCoinData(value, coinUI));

        _delay += 0.05f;
    }

    private void UpdateCoinData(int val, CoinIconUI coinIconUI)
    {
        _currencyDataService.IncreaseCoin(val);
        _poolDataService.HideObject(coinIconUI, 0f,0f);
        _delay = 0f;
    }
}