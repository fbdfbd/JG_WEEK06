using System;
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
        _commandHandler = new WeekFlowCommandHandler(_runtimeState, _presenter, _weekUiText, _weekRunner, _weekSelectionState, _weekSequenceState);
        _narrativeHandler = new WeekFlowNarrativeHandler(_runtimeState, _presenter, _weekUiText, _weekSelectionState, _weekSequenceState);
    }

    private void BindViewEvents()
    {
        if (_view == null)
        {
            return;
        }

        _view.RunWeekRequested += _commandHandler.RunCurrentWeek;
        _view.ResetSelectionsRequested += _commandHandler.ResetSelections;
        _view.ResetChildStateRequested += _commandHandler.ResetChildState;
        _view.CardOptionSelected += _commandHandler.SelectCardOption;
        _view.WeekFeedbackClosed += _narrativeHandler.CloseWeekFeedback;
        _view.WeekEventContinueRequested += _narrativeHandler.ContinueWeekEvent;
        _view.PrivateDialogueChoiceSelected += _narrativeHandler.SelectPrivateDialogueChoice;
        _view.PrivateDialogueContinueRequested += _narrativeHandler.ContinuePrivateDialogue;
    }

    private void UnbindViewEvents()
    {
        if (_view == null)
        {
            return;
        }

        _view.RunWeekRequested -= _commandHandler.RunCurrentWeek;
        _view.ResetSelectionsRequested -= _commandHandler.ResetSelections;
        _view.ResetChildStateRequested -= _commandHandler.ResetChildState;
        _view.CardOptionSelected -= _commandHandler.SelectCardOption;
        _view.WeekFeedbackClosed -= _narrativeHandler.CloseWeekFeedback;
        _view.WeekEventContinueRequested -= _narrativeHandler.ContinueWeekEvent;
        _view.PrivateDialogueChoiceSelected -= _narrativeHandler.SelectPrivateDialogueChoice;
        _view.PrivateDialogueContinueRequested -= _narrativeHandler.ContinuePrivateDialogue;
    }
}
