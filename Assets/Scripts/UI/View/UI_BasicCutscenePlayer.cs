using System.Collections;
using DG.Tweening;
using UnityEngine;

public class UI_BasicCutscenePlayer : WeekFlowCutscenePlayerBase
{
    [SerializeField] private RectTransform _target;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Vector2 _startOffset = new(0f, -80f);
    [SerializeField] private float _duration = 0.25f;
    [SerializeField] private Ease _ease = Ease.OutCubic;
    [SerializeField] private bool _useFade = true;

    private Tween _activeTween;
    private Vector2 _defaultAnchoredPosition;
    private float _defaultAlpha = 1f;
    private bool _isInitialized;

    public override bool IsPlaying => _activeTween != null && _activeTween.IsActive() && _activeTween.IsPlaying();

    private void Awake()
    {
        CacheDefaults();
    }

    private void OnDestroy()
    {
        KillTween(false);
    }

    public override IEnumerator Play(WeekFlowCutsceneRequest request)
    {
        CacheDefaults();
        KillTween(false);

        if (_target == null && _canvasGroup == null)
        {
            yield break;
        }

        Sequence sequence = DOTween.Sequence();

        if (_target != null)
        {
            _target.anchoredPosition = _defaultAnchoredPosition + _startOffset;
            sequence.Join(_target.DOAnchorPos(_defaultAnchoredPosition, _duration).SetEase(_ease));
        }

        if (_useFade && _canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
            sequence.Join(_canvasGroup.DOFade(_defaultAlpha, _duration).SetEase(_ease));
        }

        _activeTween = sequence;

        while (IsPlaying)
        {
            yield return null;
        }

        ApplyCompletedState();
        KillTween(false);
    }

    public override bool TrySkip()
    {
        if (!IsPlaying)
        {
            return false;
        }

        KillTween(true);
        ApplyCompletedState();
        return true;
    }

    public override void StopImmediate()
    {
        KillTween(false);
        RestoreDefaultState();
    }

    private void CacheDefaults()
    {
        if (_isInitialized)
        {
            return;
        }

        if (_target != null)
        {
            _defaultAnchoredPosition = _target.anchoredPosition;
        }

        if (_canvasGroup != null)
        {
            _defaultAlpha = _canvasGroup.alpha;
        }

        _isInitialized = true;
    }

    private void ApplyCompletedState()
    {
        if (_target != null)
        {
            _target.anchoredPosition = _defaultAnchoredPosition;
        }

        if (_canvasGroup != null && _useFade)
        {
            _canvasGroup.alpha = _defaultAlpha;
        }
    }

    private void RestoreDefaultState()
    {
        if (_target != null)
        {
            _target.anchoredPosition = _defaultAnchoredPosition;
        }

        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = _defaultAlpha;
        }
    }

    private void KillTween(bool complete)
    {
        if (_activeTween == null)
        {
            return;
        }

        if (_activeTween.IsActive())
        {
            _activeTween.Kill(complete);
        }

        _activeTween = null;
    }
}
