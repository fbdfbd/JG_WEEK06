using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class WeekFlowCommandHandler
{
    private readonly WeekFlowRuntimeState _runtimeState;
    private readonly WeekFlowPresenter _presenter;
    private readonly WeekUiTextProvider _weekUiText;
    private readonly WeekRunner _weekRunner;
    private readonly WeekSelectionState _weekSelectionState;
    private readonly WeekSequenceState _weekSequenceState;

    public WeekFlowCommandHandler(
        WeekFlowRuntimeState runtimeState,
        WeekFlowPresenter presenter,
        WeekUiTextProvider weekUiText,
        WeekRunner weekRunner,
        WeekSelectionState weekSelectionState,
        WeekSequenceState weekSequenceState)
    {
        _runtimeState = runtimeState;
        _presenter = presenter;
        _weekUiText = weekUiText;
        _weekRunner = weekRunner;
        _weekSelectionState = weekSelectionState;
        _weekSequenceState = weekSequenceState;
    }

    public void RunCurrentWeek()
    {
        if (_runtimeState.HasReachedEnding)
        {
            PublishStatusMessage(_weekUiText.GetEndingAlreadyReachedMessage());
            return;
        }

        SO_WeekDefinition currentWeekDefinition = _weekSequenceState.CurrentWeekDefinition;
        if (currentWeekDefinition == null)
        {
            PublishStatusMessage(_weekUiText.GetWeekDefinitionMissingMessage());
            return;
        }

        try
        {
            Dictionary<EChildStatusType, int> previousStats = WeekFlowQueryUtility.CaptureCurrentStats(_runtimeState.ChildState);
            _runtimeState.ChildState.ClearReactionLogs();

            RuntimeWeekSelection[] selections = _weekSelectionState.BuildSelections(
                WeekFlowQueryUtility.GetCurrentWeekEntries(currentWeekDefinition));

            _runtimeState.LastWeekResult = _weekRunner.RunWeek(currentWeekDefinition, _runtimeState.ChildState, selections);

            WeekFeedbackPresentation feedbackPresentation = WeekFeedbackResolver.Resolve(
                currentWeekDefinition,
                _runtimeState.LastWeekResult,
                _runtimeState.ChildState,
                previousStats);

            _runtimeState.PendingWeekEvent = WeekNarrativeResolver.ResolveFixedEvent(currentWeekDefinition, _runtimeState.ChildState, _weekUiText);
            _runtimeState.PendingPrivateDialogue = WeekNarrativeResolver.ResolvePrivateDialogue(currentWeekDefinition, _weekUiText);
            _runtimeState.SelectedDialogueChoice = default;
            _runtimeState.HasAppliedPendingWeekEvent = false;
            _runtimeState.HasSelectedDialogueChoice = false;
            _runtimeState.ShouldShowEndingAfterNarrative = _weekSequenceState.IsCurrentWeekFinal;
            _runtimeState.ShouldAdvanceToNextWeekAfterNarrative = !_runtimeState.ShouldShowEndingAfterNarrative;

            PublishStatusMessage(_weekUiText.GetWeekCompletedMessage(currentWeekDefinition.WeekIndex));
            _presenter.RefreshAll();
            _presenter.ShowWeekFeedback(feedbackPresentation);
        }
        catch (Exception exception)
        {
            PublishStatusMessage(_weekUiText.GetWeekExecutionFailedMessage(exception.Message));
            Debug.LogError(exception);
        }
    }

    public void ResetSelections()
    {
        _weekSelectionState.ResetAllSelections(
            WeekFlowQueryUtility.GetCurrentWeekEntries(_weekSequenceState.CurrentWeekDefinition));
        _runtimeState.LastWeekResult = null;

        PublishStatusMessage(_weekUiText.GetAllSelectionsResetMessage());
        _presenter.RefreshAll();
    }

    public void ResetChildState()
    {
        _runtimeState.ResetChildState();

        PublishStatusMessage(_weekUiText.GetChildStateResetMessage());
        _presenter.RefreshAll();
        _presenter.PublishDefaultNemoFeedback();
    }

    public void SelectCardOption(SO_CardInfoDefinition cardDefinition, int optionIndex)
    {
        bool hasChangedSelection = _weekSelectionState.TrySetSelectedOptionIndex(cardDefinition, optionIndex);
        if (!hasChangedSelection)
        {
            return;
        }

        PublishStatusMessage(_weekUiText.GetCardSelectionUpdatedMessage());
        _presenter.PublishSelectionEntries();
    }

    private void PublishStatusMessage(string statusMessage)
    {
        _runtimeState.SetStatusMessage(statusMessage);
        _presenter.PublishStatusMessage();
    }
}
