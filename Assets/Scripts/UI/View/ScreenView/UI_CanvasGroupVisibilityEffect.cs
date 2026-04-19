using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class UI_CanvasGroupVisibilityEffect : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _root;
    [SerializeField] private bool _disableGameObjectOnClose;
    [SerializeField] private bool _ignoreTimeScale = true;
    [SerializeField] private Vector2 _moveFrom = new Vector2(-40f, 0f);
    [SerializeField] private float _openDuration = 0.22f;
    [SerializeField] private float _closeDuration = 0.18f;
    [SerializeField] private Ease _openEase = Ease.OutCubic;
    [SerializeField] private Ease _closeEase = Ease.InCubic;

    private bool _initialized;
    private bool _hasShownState;
    private Vector2 _shownAnchoredPosition;
    private Sequence _sequence;

    private void Reset()
    {
        _target = gameObject;
        _root = GetComponent<RectTransform>();

        if (_canvasGroup == null)
        {
            TryGetComponent(out _canvasGroup);
        }
    }

    private void Awake()
    {
        Initialize();
    }

    public void Open()
    {
        Initialize();
        KillSequence();

        if (_target != null && !_target.activeSelf)
        {
            _target.SetActive(true);
        }

        ApplyHiddenInstant();

        _sequence = DOTween.Sequence()
            .SetUpdate(UpdateType.Late, _ignoreTimeScale)
            .Join(_root.DOAnchorPos(_shownAnchoredPosition, _openDuration).SetEase(_openEase));

        if (_canvasGroup != null)
        {
            _sequence.Join(_canvasGroup.DOFade(1f, _openDuration * 0.9f).SetEase(Ease.OutQuad))
                .OnComplete(() => _canvasGroup.blocksRaycasts = true);
        }
    }

    public void Close()
    {
        Initialize();

        KillSequence();

        Vector2 hiddenPosition = _shownAnchoredPosition + _moveFrom;

        _sequence = DOTween.Sequence()
            .SetUpdate(UpdateType.Late, _ignoreTimeScale)
            .Join(_root.DOAnchorPos(hiddenPosition, _closeDuration).SetEase(_closeEase));

        if (_canvasGroup != null)
        {
            _canvasGroup.blocksRaycasts = false;
            _sequence.Join(_canvasGroup.DOFade(0f, _closeDuration * 0.85f).SetEase(Ease.InQuad));
        }

        _sequence.OnComplete(HandleCloseCompleted);
    }

    private void OnDisable()
    {
        KillSequence();
    }

    private void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        if (_target == null)
        {
            _target = gameObject;
        }

        if (_root == null)
        {
            _root = _target.GetComponent<RectTransform>();
        }

        if (_canvasGroup == null)
        {
            _target.TryGetComponent(out _canvasGroup);
        }

        if (!_hasShownState)
        {
            _shownAnchoredPosition = _root != null ? _root.anchoredPosition : Vector2.zero;
            _hasShownState = true;
        }

        _initialized = true;
    }

    private void ApplyHiddenInstant()
    {
        if (_root != null)
        {
            _root.anchoredPosition = _shownAnchoredPosition + _moveFrom;
        }

        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
        }
    }

    private void HandleCloseCompleted()
    {
        if (_disableGameObjectOnClose && _target != null)
        {
            if (_root != null)
            {
                _root.anchoredPosition = _shownAnchoredPosition;
            }

            _target.SetActive(false);
        }
    }

    private void KillSequence()
    {
        if (_sequence != null)
        {
            _sequence.Kill();
            _sequence = null;
        }
    }
}
