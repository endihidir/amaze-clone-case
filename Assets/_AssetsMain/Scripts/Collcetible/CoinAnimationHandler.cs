using System;
using DG.Tweening;
using UnityBase.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinAnimationHandler : MonoBehaviour
{
    private Camera _cam;
    private Vector3 _defaultPos, _defaultScale;
    private Tween _coinAnimTween;
    public bool IsActive => isActiveAndEnabled;
    
    private void Awake()
    {
        _cam = Camera.main;
        _defaultPos = transform.localPosition;
        _defaultScale = transform.localScale;
    }

    public void StartIdleAnim()
    {
        KillAnim();
        
        var startDelay = Random.Range(0f, 0.5f);

        _coinAnimTween = DOTween.Sequence()
                                .AppendInterval(0.5f)
                                .Append(transform.DOLocalMoveZ(_defaultPos.z + 0.1f, 0.2f).SetEase(Ease.OutCubic).SetDelay(startDelay))
                                .SetLoops(-1, LoopType.Yoyo);
    }

    public void StartMoveAnim(Vector3 targetPos, float startDelay, Action onComplete)
    {
        KillAnim();
        
        var pos = _cam.ScreenToWorldPoint(targetPos);

        transform.localPosition = transform.localPosition.With(y: 5f, z: -1f);

        _coinAnimTween = DOTween.Sequence()
                                .AppendInterval(startDelay)
                                .Append(transform.DOMove(pos, 0.75f).SetEase(Ease.InBack))
                                .Join(transform.DOScale(0.6f, 0.75f).SetEase(Ease.OutBack))
                                .AppendCallback(() =>
                                {
                                    gameObject.SetActive(false);
                                    transform.localPosition = _defaultPos;
                                    transform.localScale = _defaultScale;
                                    onComplete?.Invoke();
                                });
    }

    public void Reset()
    {
        gameObject.SetActive(true);
    }

    private void KillAnim() => _coinAnimTween?.Kill();
    private void OnDestroy() => KillAnim();
}