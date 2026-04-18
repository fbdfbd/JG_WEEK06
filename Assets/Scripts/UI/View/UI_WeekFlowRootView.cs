using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_WeekFlowRootView : WeekFlowViewBase
{
    private enum EDialogueContinueRoute
    {
        None,
        WeekFeedback,
        Narrative
    }

    [Header("Panels")]
    [SerializeField] private UI_WeekFlowScreenView _weekScreenView;
    [SerializeField] private UI_DialogueScreenView _dialogueScreenView;
    [SerializeField] private UI_DialogueLogPanel _dialogueLogPanel;
    [SerializeField] private UI_WeekFlowTransitionPlayer _transitionPlayer;
    [SerializeField] private WeekFlowCutsceneBridgeBase _cutsceneBridge;

    [Header("Log")]
    [SerializeField] private Button _openLogButton;

    private EDialogueContinueRoute _dialogueContinueRoute;
    private readonly WeekFlowDialogueLogService _dialogueLogService = new();

    private void Awake()
    {
        InitializePanels();
        BindWeekScreenEvents();
        BindDialogueScreenEvents();
        BindLogEvents();
        HideTransientViews();
    }

    private void OnDestroy()
    {
        UnbindWeekScreenEvents();
        UnbindDialogueScreenEvents();
        UnbindLogEvents();
    }

    public override void RenderWeekHeader(WeekHeaderPresentation presentation)
    {
        if (_weekScreenView == null)
        {
            return;
        }

        _weekScreenView.RenderWeekHeader(presentation);
    }

    public override void RenderSelectionGroups(IReadOnlyList<WeekSelectionCategoryGroupPresentation> groups)
    {
        if (_weekScreenView == null)
        {
            return;
        }

        _weekScreenView.RenderSelectionGroups(groups);
    }

    public override void RenderChildState(ChildStatePresentation presentation)
    {
        if (_weekScreenView == null)
        {
            return;
        }

        _weekScreenView.RenderChildState(presentation);
    }

    public override void ShowWeekFeedback(WeekFeedbackPresentation presentation)
    {
        if (_dialogueScreenView == null)
        {
            return;
        }

        _dialogueContinueRoute = EDialogueContinueRoute.WeekFeedback;
        _dialogueScreenView.ShowWeekFeedback(presentation);
    }

    public override void ShowInteractiveEvent(InteractiveEventPresentation presentation)
    {
        if (_dialogueScreenView == null)
        {
            return;
        }

        _dialogueContinueRoute = EDialogueContinueRoute.Narrative;
        _dialogueScreenView.ShowInteractiveEvent(presentation);
    }

    public override void ShowInteractiveEventResult(InteractiveEventChoiceResultPresentation presentation)
    {
        if (_dialogueScreenView == null)
        {
            return;
        }

        _dialogueContinueRoute = EDialogueContinueRoute.Narrative;
        _dialogueScreenView.ShowInteractiveEventResult(presentation);
    }

    public override void ShowEnding(EndingPresentation presentation)
    {
        if (_dialogueScreenView == null)
        {
            return;
        }

        _dialogueContinueRoute = EDialogueContinueRoute.Narrative;
        _dialogueScreenView.ShowEnding(presentation);
    }

    public override void HideTransientViews()
    {
        _dialogueContinueRoute = EDialogueContinueRoute.None;

        if (_dialogueScreenView == null)
        {
            return;
        }

        _dialogueScreenView.HideView();
    }

    public override WeekFlowCutsceneBridgeBase GetCutsceneBridge()
    {
        return _cutsceneBridge;
    }

    public override void SetFlowScreenContext(WeekFlowScreen screen, RuntimeChildState childState, RuntimeWeekResult lastWeekResult)
    {
        if (_dialogueScreenView == null)
        {
            return;
        }

        _dialogueScreenView.SetScreenContext(screen, childState, lastWeekResult);
    }

    public override System.Collections.IEnumerator PlayCurrentDialogueCutscene()
    {
        if (_dialogueScreenView == null)
        {
            yield break;
        }

        yield return _dialogueScreenView.PlayCurrentDialogueCutscene();
    }

    public override System.Collections.IEnumerator PlayFlowTransition(WeekFlowTransitionContext context)
    {
        if (_transitionPlayer == null)
        {
            yield break;
        }

        yield return _transitionPlayer.Play(context);
    }

    private void InitializePanels()
    {
        if (_dialogueScreenView == null)
        {
            return;
        }

        _dialogueScreenView.SetDialogueLogService(_dialogueLogService);
        _dialogueScreenView.SetCutsceneBridge(_cutsceneBridge);

        if (_dialogueLogPanel != null)
        {
            _dialogueLogPanel.SetDialogueLogService(_dialogueLogService);
            _dialogueLogPanel.Hide();
        }
    }

    private void BindWeekScreenEvents()
    {
        if (_weekScreenView == null)
        {
            return;
        }

        _weekScreenView.RunWeekRequested += HandleRunWeekRequested;
        _weekScreenView.CardOptionSelected += HandleCardOptionSelected;
    }

    private void UnbindWeekScreenEvents()
    {
        if (_weekScreenView == null)
        {
            return;
        }

        _weekScreenView.RunWeekRequested -= HandleRunWeekRequested;
        _weekScreenView.CardOptionSelected -= HandleCardOptionSelected;
    }

    private void BindDialogueScreenEvents()
    {
        if (_dialogueScreenView == null)
        {
            return;
        }

        _dialogueScreenView.ContinueRequested += HandleDialogueContinueRequested;
        _dialogueScreenView.ChoiceSelected += HandleDialogueChoiceSelected;
    }

    private void BindLogEvents()
    {
        if (_openLogButton == null)
        {
            return;
        }

        _openLogButton.onClick.AddListener(HandleOpenLogButtonClicked);
    }

    private void UnbindDialogueScreenEvents()
    {
        if (_dialogueScreenView == null)
        {
            return;
        }

        _dialogueScreenView.ContinueRequested -= HandleDialogueContinueRequested;
        _dialogueScreenView.ChoiceSelected -= HandleDialogueChoiceSelected;
    }

    private void UnbindLogEvents()
    {
        if (_openLogButton == null)
        {
            return;
        }

        _openLogButton.onClick.RemoveListener(HandleOpenLogButtonClicked);
    }

    private void HandleRunWeekRequested()
    {
        RaiseRunWeekRequested();
    }

    private void HandleOpenLogButtonClicked()
    {
        if (_dialogueLogPanel == null)
        {
            return;
        }

        _dialogueLogPanel.Show();
    }

    private void HandleCardOptionSelected(SO_CardInfoDefinition cardDefinition, int optionIndex)
    {
        RaiseCardOptionSelected(cardDefinition, optionIndex);
    }

    private void HandleDialogueContinueRequested()
    {
        if (TrySkipCurrentTransition())
        {
            return;
        }

        switch (_dialogueContinueRoute)
        {
            case EDialogueContinueRoute.WeekFeedback:
                RaiseWeekFeedbackClosed();
                break;

            case EDialogueContinueRoute.Narrative:
                RaiseInteractiveEventContinueRequested();
                break;
        }
    }

    private void HandleDialogueChoiceSelected(int choiceIndex)
    {
        if (TrySkipCurrentTransition())
        {
            return;
        }

        RaiseInteractiveEventChoiceSelected(choiceIndex);
    }

    private bool TrySkipCurrentTransition()
    {
        if (_transitionPlayer == null)
        {
            return false;
        }

        return _transitionPlayer.TrySkipCurrent();
    }
}

public abstract class WeekFlowCutscenePlayerBase : MonoBehaviour
{
    [SerializeField] private string _cutsceneId = string.Empty;
    [SerializeField] private bool _isBlocking = true;

    public string CutsceneId => _cutsceneId;
    public bool IsBlocking => _isBlocking;
    public abstract bool IsPlaying { get; }

    public virtual bool CanPlay(WeekFlowCutsceneRequest request)
    {
        return true;
    }

    public abstract IEnumerator Play(WeekFlowCutsceneRequest request);
    public abstract bool TrySkip();
    public abstract void StopImmediate();
}

public class UI_WeekFlowCutsceneBridge : WeekFlowCutsceneBridgeBase
{
    [SerializeField] private SO_WeekFlowCutsceneCatalog _catalog;
    [SerializeField] private WeekFlowCutscenePlayerBase[] _players;

    private readonly Dictionary<string, WeekFlowCutscenePlayerBase> _playerLookup = new();
    private readonly WeekFlowCutsceneResolver _resolver = new();
    private WeekFlowCutscenePlayerBase _activePlayer;

    public override bool IsPlaying => _activePlayer != null && _activePlayer.IsPlaying;
    public override bool IsBlocking => IsPlaying && _activePlayer.IsBlocking;

    private void Awake()
    {
        CachePlayers();
    }

    private void OnValidate()
    {
        CachePlayers();
    }

    public override IEnumerator Play(WeekFlowCutsceneRequest request)
    {
        CachePlayers();

        if (!_resolver.TryResolveCutsceneId(request, _catalog, out string cutsceneId))
        {
            yield break;
        }

        if (!_playerLookup.TryGetValue(cutsceneId, out WeekFlowCutscenePlayerBase player) || player == null)
        {
            yield break;
        }

        if (!player.CanPlay(request))
        {
            yield break;
        }

        if (_activePlayer != null && _activePlayer != player)
        {
            _activePlayer.StopImmediate();
            _activePlayer = null;
        }

        _activePlayer = player;
        yield return player.Play(request);

        if (_activePlayer == player)
        {
            _activePlayer = null;
        }
    }

    public override bool TrySkip()
    {
        if (_activePlayer == null)
        {
            return false;
        }

        return _activePlayer.TrySkip();
    }

    public override void StopImmediate()
    {
        if (_activePlayer == null)
        {
            return;
        }

        _activePlayer.StopImmediate();
        _activePlayer = null;
    }

    private void CachePlayers()
    {
        _playerLookup.Clear();

        if (_players == null || _players.Length == 0)
        {
            _players = GetComponentsInChildren<WeekFlowCutscenePlayerBase>(true);
        }

        for (int index = 0; index < _players.Length; index++)
        {
            WeekFlowCutscenePlayerBase player = _players[index];
            if (player == null || string.IsNullOrWhiteSpace(player.CutsceneId))
            {
                continue;
            }

            _playerLookup[player.CutsceneId] = player;
        }
    }
}

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

