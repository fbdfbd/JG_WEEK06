using System;
using System.Collections;
using UnityEngine;

public class WeekFlowController : MonoBehaviour
{
    [Header("Week Data")]
    [SerializeField] private SO_WeekDefinition _weekDefinition;
    [SerializeField] private SO_WeekDefinition[] _weekDefinitions = Array.Empty<SO_WeekDefinition>();
    [SerializeField] private SO_WeekUiTextCatalog _uiTextCatalog;

    [Header("View Connection")]
    [SerializeField] private WeekFlowViewBase _view;

    private readonly WeekRunner _weekRunner = new();
    private readonly WeekSelectionState _weekSelectionState = new();
    private readonly WeekSequenceState _weekSequenceState = new();

    private WeekFlowRuntimeState _runtimeState;
    private WeekUiTextProvider _weekUiText;
    private WeekFlowPresenter _presenter;
    private WeekFlowCommandHandler _commandHandler;
    private WeekFlowNarrativeHandler _narrativeHandler;
    private WeekFlowCinematicDirector _cinematicDirector;
    private WeekFlowScreen _currentScreen;
    private bool _isTransitionPlaying;

    protected virtual void Awake()
    {
        ResolveConnectedView();
        BuildWeekFlowObjects();
        BindViewEvents();
        _presenter.RefreshAll();
        _presenter.PublishDefaultNemoFeedback();
    }

    protected virtual void OnDestroy()
    {
        UnbindViewEvents();
    }

    private void ResolveConnectedView()
    {
        if (_view == null)
        {
            _view = GetComponent<WeekFlowViewBase>();
        }
    }

    private void BuildWeekFlowObjects()
    {
        _runtimeState = new WeekFlowRuntimeState();
        _weekUiText = new WeekUiTextProvider(_uiTextCatalog);
        _weekSequenceState.InitializeWeekSequence(_weekDefinition, _weekDefinitions);
        _weekSelectionState.ApplyWeekEntries(WeekFlowQueryUtility.GetCurrentWeekEntries(_weekSequenceState.CurrentWeekDefinition));
        _presenter = new WeekFlowPresenter(_view, _runtimeState, _weekUiText, _weekSelectionState, _weekSequenceState);
        _commandHandler = new WeekFlowCommandHandler(_runtimeState, _weekUiText, _weekRunner, _weekSelectionState, _weekSequenceState);
        _narrativeHandler = new WeekFlowNarrativeHandler(_runtimeState, _weekUiText, _weekSelectionState, _weekSequenceState);
        _cinematicDirector = new WeekFlowCinematicDirector(_view, new WeekFlowCinematicResolver());
    }

    private void BindViewEvents()
    {
        if (_view == null)
        {
            return;
        }

        _view.RunWeekRequested += HandleRunWeekRequested;
        _view.ResetSelectionsRequested += HandleResetSelectionsRequested;
        _view.ResetChildStateRequested += HandleResetChildStateRequested;
        _view.CardOptionSelected += HandleCardOptionSelected;
        _view.WeekFeedbackClosed += HandleWeekFeedbackClosed;
        _view.InteractiveEventContinueRequested += HandleInteractiveEventContinueRequested;
        _view.InteractiveEventChoiceSelected += HandleInteractiveEventChoiceSelected;
    }

    private void UnbindViewEvents()
    {
        if (_view == null)
        {
            return;
        }

        _view.RunWeekRequested -= HandleRunWeekRequested;
        _view.ResetSelectionsRequested -= HandleResetSelectionsRequested;
        _view.ResetChildStateRequested -= HandleResetChildStateRequested;
        _view.CardOptionSelected -= HandleCardOptionSelected;
        _view.WeekFeedbackClosed -= HandleWeekFeedbackClosed;
        _view.InteractiveEventContinueRequested -= HandleInteractiveEventContinueRequested;
        _view.InteractiveEventChoiceSelected -= HandleInteractiveEventChoiceSelected;
    }

    private void HandleRunWeekRequested() => RunFlowAction(_commandHandler.RunCurrentWeek);
    private void HandleResetSelectionsRequested() => RunFlowAction(_commandHandler.ResetSelections);
    private void HandleResetChildStateRequested() => RunFlowAction(_commandHandler.ResetChildState);
    private void HandleWeekFeedbackClosed() => RunFlowAction(_narrativeHandler.CloseWeekFeedback);
    private void HandleInteractiveEventContinueRequested() => RunFlowAction(_narrativeHandler.ContinueInteractiveEvent);
    private void HandleCardOptionSelected(SO_CardInfoDefinition cardDefinition, int optionIndex) => RunFlowAction(() => _commandHandler.SelectCardOption(cardDefinition, optionIndex));
    private void HandleInteractiveEventChoiceSelected(int choiceIndex) => RunFlowAction(() => _narrativeHandler.SelectInteractiveEventChoice(choiceIndex));

    private void RunFlowAction(Func<WeekFlowActionResult> action)
    {
        if (_isTransitionPlaying)
        {
            return;
        }

        SO_WeekDefinition previousWeek = _weekSequenceState.CurrentWeekDefinition;
        WeekFlowActionResult result = action();
        if (!result.ShouldRefreshUi)
        {
            return;
        }

        StartCoroutine(ApplyFlowAction(result, previousWeek, _weekSequenceState.CurrentWeekDefinition));
    }

    private IEnumerator ApplyFlowAction(WeekFlowActionResult result, SO_WeekDefinition previousWeek, SO_WeekDefinition currentWeek)
    {
        _isTransitionPlaying = true;

        if (result.ShouldReplaceScreen && _currentScreen != null)
        {
            yield return _cinematicDirector.PlayScreenExit(_currentScreen);
            _presenter.HideFlowScreens();
        }

        if (previousWeek != currentWeek)
        {
            yield return _cinematicDirector.PlayWeekChangeOut(previousWeek);
        }

        _presenter.RefreshAll();

        if (previousWeek != currentWeek)
        {
            yield return _cinematicDirector.PlayWeekChangeIn(currentWeek);
        }

        if (result.ShouldReplaceScreen)
        {
            _currentScreen = result.NextScreen;
            if (_currentScreen != null)
            {
                _presenter.PresentScreen(_currentScreen);
                _presenter.PublishNemoFeedback(_currentScreen.NemoFeedback);
                yield return _cinematicDirector.PlayScreenEnter(_currentScreen);
            }
        }

        _isTransitionPlaying = false;
    }
}
