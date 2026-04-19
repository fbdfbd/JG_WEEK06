using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class UI_CardShowEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform root;      // 위치 이동용
    [SerializeField] private RectTransform visual;    // 스케일 연출용 (없으면 root 사용)
    [SerializeField] private CanvasGroup canvasGroup; // 페이드용

    [Header("Behavior")]
    [SerializeField] private bool playOpenOnEnable = true;
    [SerializeField] private bool ignoreTimeScale = true;

    [Header("Animation")]
    [Tooltip("왼쪽 아래에서 스르륵 올라오는 느낌용 UI 오프셋")]
    [SerializeField] private Vector2 moveFrom = new Vector2(-40f, -40f);

    [Tooltip("닫힌 상태의 스케일")]
    [SerializeField] private Vector3 hiddenScale = new Vector3(0.92f, 0.92f, 1f);

    [SerializeField] private float openDuration = 0.28f;
    [SerializeField] private float closeDuration = 0.20f;

    [SerializeField] private Ease openEase = Ease.OutCubic;
    [SerializeField] private Ease closeEase = Ease.InCubic;

    [Header("Events")]
    [SerializeField] private UnityEvent onOpened;
    [SerializeField] private UnityEvent onClosed;

    private Sequence _sequence;

    private Vector2 _shownAnchoredPosition;
    private Vector3 _shownScale;

    private bool _initialized;
    private bool _hasShownState;

    private void Reset()
    {
        root = GetComponent<RectTransform>();
        visual = root;

        if (!TryGetComponent(out canvasGroup))
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        Initialize();

        if (playOpenOnEnable)
            PlayOpen();
        else
            ApplyShownInstant();
    }

    private void OnDisable()
    {
        KillSequence();
    }

    private void Initialize()
    {
        if (_initialized)
            return;

        if (root == null)
            root = GetComponent<RectTransform>();

        if (visual == null)
            visual = root;

        if (canvasGroup == null)
        {
            if (!TryGetComponent(out canvasGroup))
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (!_hasShownState)
        {
            _shownAnchoredPosition = root.anchoredPosition;
            _shownScale = visual.localScale == Vector3.zero ? Vector3.one : visual.localScale;
            _hasShownState = true;
        }

        _initialized = true;
    }

    [ContextMenu("Capture Current As Shown State")]
    public void CaptureCurrentAsShownState()
    {
        Initialize();

        _shownAnchoredPosition = root.anchoredPosition;
        _shownScale = visual.localScale == Vector3.zero ? Vector3.one : visual.localScale;
        _hasShownState = true;
    }

    [ContextMenu("Open")]
    public void PlayOpen()
    {
        Initialize();
        KillSequence();

        ApplyHiddenInstant();

        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        _sequence = DOTween.Sequence()
            .SetUpdate(UpdateType.Late, ignoreTimeScale)
            .Join(root.DOAnchorPos(_shownAnchoredPosition, openDuration).SetEase(openEase))
            .Join(visual.DOScale(_shownScale, openDuration).SetEase(openEase))
            .Join(canvasGroup.DOFade(1f, openDuration * 0.9f).SetEase(Ease.OutQuad))
            .OnComplete(() =>
            {
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;
                onOpened?.Invoke();
            });
    }

    [ContextMenu("Close Only")]
    public void PlayClose()
    {
        PlayCloseInternal(false);
    }

    public void Close()
    {
        PlayCloseInternal(true);
    }

    public void Open()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true); // OnEnable -> PlayOpen
            return;
        }

        PlayOpen();
    }

    private void PlayCloseInternal(bool disableAfterClose)
    {
        Initialize();
        KillSequence();

        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        Vector2 hiddenPosition = _shownAnchoredPosition + moveFrom;

        _sequence = DOTween.Sequence()
            .SetUpdate(UpdateType.Late, ignoreTimeScale)
            .Join(root.DOAnchorPos(hiddenPosition, closeDuration).SetEase(closeEase))
            .Join(visual.DOScale(hiddenScale, closeDuration).SetEase(closeEase))
            .Join(canvasGroup.DOFade(0f, closeDuration * 0.85f).SetEase(Ease.InQuad))
            .OnComplete(() =>
            {
                onClosed?.Invoke();

                if (disableAfterClose)
                {
                    // 다음 Open 때 기준점이 틀어지지 않게 shown 상태를 복구한 뒤 비활성화
                    ApplyShownInstant();
                    gameObject.SetActive(false);
                }
            });
    }

    public void ApplyShownInstant()
    {
        Initialize();

        root.anchoredPosition = _shownAnchoredPosition;
        visual.localScale = _shownScale;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    public void ApplyHiddenInstant()
    {
        Initialize();

        root.anchoredPosition = _shownAnchoredPosition + moveFrom;
        visual.localScale = hiddenScale;

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
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