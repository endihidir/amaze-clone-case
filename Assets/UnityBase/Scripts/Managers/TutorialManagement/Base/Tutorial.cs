using System;
using DG.Tweening;
using UnityBase.Extensions;
using UnityBase.Pool;
using UnityBase.Service;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace UnityBase.TutorialCore
{
    public abstract class Tutorial : MonoBehaviour, IPoolable
    {
        #region VARIABLES

        [Header("Tutorial Contents")] 
        [SerializeField] private PositionSpace _spawnSpace;

        [SerializeField] private Graphic[] _graphics;

        private Tween[] _fadeTweens;
        private event Action _onFadeComplete;
        protected event Action _onHideComplete;

        #endregion

        #region PROPERTIES

        public Component PoolableObject => this;
        public bool IsActive => isActiveAndEnabled;
        public bool IsUnique => false;
        public PositionSpace SpawnSpace => _spawnSpace;

        [Inject] 
        private readonly ITutorialDataService _tutorialDataService;

        #endregion

#if UNITY_EDITOR
        protected virtual void OnValidate() => _graphics = GetComponentsInChildren<Graphic>();
#endif

        protected virtual void Awake()
        {
            _fadeTweens = new Tween[_graphics.Length];

            CashDefaultTransformData();
        }

        public void Show(float duration = 0f, float delay = 0f)
        {
            gameObject.SetActive(true);
        }
        
        public void Hide(float duration = 0f, float delay = 0f)
        {
            if (duration + delay > 0f)
            {
                SmoothFade(0f, duration, delay).OnFadeComplete(ResetTutorial);
            }
            else
            {
                Fade(0f);
                ResetTutorial();
            }
        }
        
        public void SetSpawnSpace(PositionSpace spawnSpace) => _spawnSpace = spawnSpace;
        public void OnHideComplete(Action act) => _onHideComplete = act;
        public void InvokeHideComplete() => _onHideComplete?.Invoke();

        protected virtual void ResetTutorial()
        {
            gameObject.SetActive(false);

            SetDefaultTransformData();

            Fade(1f);

            InvokeHideComplete();
        }

        private Tutorial SmoothFade(float endVal, float duration, float delay, Ease ease = Ease.Linear)
        {
            var graphicsLenght = _graphics.Length;

            var counter = 0;

            for (int i = 0; i < graphicsLenght; i++)
            {
                _fadeTweens[i] = _graphics[i].DOFade(endVal, duration).SetDelay(delay)
                    .SetEase(ease)
                    .OnComplete(() => OnAllFadeComplete(ref counter, graphicsLenght));
            }

            return this;
        }

        private void OnAllFadeComplete(ref int counter, int graphicsLenght)
        {
            counter++;

            if (counter >= graphicsLenght)
            {
                _onFadeComplete?.Invoke();
            }
        }

        private void OnFadeComplete(Action act) => _onFadeComplete = act;

        private void Fade(float alpha)
        {
            var graphicsLenght = _graphics.Length;

            for (int i = 0; i < graphicsLenght; i++)
            {
                _graphics[i].color = _graphics[i].color.SetAlpha(alpha);
            }
        }

        private void KillFadeTweens()
        {
            var graphicsLenght = _graphics.Length;

            for (int i = 0; i < graphicsLenght; i++)
            {
                _fadeTweens[i].Kill();
            }
        }

        protected virtual void OnDestroy()
        {
            KillFadeTweens();

            _tutorialDataService.RemoveTutorial(this);
        }

        protected abstract void SetDefaultTransformData();
        protected abstract void CashDefaultTransformData();
    }
}