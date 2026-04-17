using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class WeekFlowCommandHandler
{
    private readonly WeekFlowRuntimeState _runtimeState;
    private readonly WeekUiTextProvider _weekUiText;
    private readonly WeekRunner _weekRunner;
    private readonly WeekSelectionState _weekSelectionState;
    private readonly WeekSequenceState _weekSequenceState;

    public WeekFlowCommandHandler(
        WeekFlowRuntimeState runtimeState,
        WeekUiTextProvider weekUiText,
        WeekRunner weekRunner,
        WeekSelectionState weekSelectionState,
        WeekSequenceState weekSequenceState)
    {
        _runtimeState = runtimeState;
        _weekUiText = weekUiText;
        _weekRunner = weekRunner;
        _weekSelectionState = weekSelectionState;
        _weekSequenceState = weekSequenceState;
    }

    public WeekFlowActionResult RunCurrentWeek()
    {
        if (_runtimeState.HasReachedEnding)
        {
            PublishStatusMessage(_weekUiText.GetEndingAlreadyReachedMessage());
            return WeekFlowActionResult.RefreshOnly();
        }

        SO_WeekDefinition currentWeekDefinition = _weekSequenceState.CurrentWeekDefinition;
        if (currentWeekDefinition == null)
        {
            PublishStatusMessage(_weekUiText.GetWeekDefinitionMissingMessage());
            return WeekFlowActionResult.RefreshOnly();
        }

        try
        {
            Dictionary<EChildStatusType, int> previousStats = WeekFlowQueryUtility.CaptureCurrentStats(_runtimeState.ChildState);
            _runtimeState.ChildState.ClearReactionLogs();

            RuntimeWeekSelection[] selections = _weekSelectionState.BuildSelections(
                WeekFlowQueryUtility.GetCurrentWeekEntries(currentWeekDefinition));
            _runtimeState.LastWeekResult = _weekRunner.RunWeek(currentWeekDefinition, _runtimeState.ChildState, selections);

            WeekFeedbackPresentation feedback = WeekFeedbackResolver.Resolve(
                currentWeekDefinition,
                _runtimeState.LastWeekResult,
                _runtimeState.ChildState,
                previousStats);

            _runtimeState.SetPendingEvents(WeekNarrativeResolver.ResolvePendingEvents(
                currentWeekDefinition,
                _runtimeState.ChildState,
                _runtimeState.LastWeekResult.InformationControlResult));
            _runtimeState.ShouldShowEndingAfterEvents = _weekSequenceState.IsCurrentWeekFinal;
            _runtimeState.ShouldAdvanceToNextWeekAfterEvents = !_runtimeState.ShouldShowEndingAfterEvents;

            PublishStatusMessage(_weekUiText.GetWeekCompletedMessage(currentWeekDefinition.WeekIndex));
            return WeekFlowActionResult.ReplaceScreen(WeekFlowScreen.CreateWeekFeedback(
                currentWeekDefinition,
                feedback,
                ResolveWeekFeedbackNemo()));
        }
        catch (Exception exception)
        {
            PublishStatusMessage(_weekUiText.GetWeekExecutionFailedMessage(exception.Message));
            Debug.LogError(exception);
            return WeekFlowActionResult.RefreshOnly();
        }
    }

    public WeekFlowActionResult ResetSelections()
    {
        _weekSelectionState.ResetAllSelections(
            WeekFlowQueryUtility.GetCurrentWeekEntries(_weekSequenceState.CurrentWeekDefinition));
        _runtimeState.LastWeekResult = null;
        PublishStatusMessage(_weekUiText.GetAllSelectionsResetMessage());
        return WeekFlowActionResult.ClearScreen();
    }

    public WeekFlowActionResult ResetChildState()
    {
        _runtimeState.ResetChildState();
        PublishStatusMessage(_weekUiText.GetChildStateResetMessage());
        return WeekFlowActionResult.ClearScreen();
    }

    public WeekFlowActionResult SelectCardOption(SO_CardInfoDefinition cardDefinition, int optionIndex)
    {
        if (!_weekSelectionState.TrySetSelectedOptionIndex(cardDefinition, optionIndex))
        {
            return WeekFlowActionResult.None;
        }

        PublishStatusMessage(_weekUiText.GetCardSelectionUpdatedMessage());
        return WeekFlowActionResult.RefreshOnly();
    }

    private NemoFeedbackPresentation ResolveWeekFeedbackNemo()
    {
        RuntimeResolvedCardRecord lastResolvedCard = _runtimeState.LastWeekResult.ResolvedCards.Count > 0
            ? _runtimeState.LastWeekResult.ResolvedCards[_runtimeState.LastWeekResult.ResolvedCards.Count - 1]
            : null;
        return NemoFeedbackResolver.Resolve(_runtimeState.ChildState, lastResolvedCard);
    }

    private void PublishStatusMessage(string statusMessage)
    {
        _runtimeState.SetStatusMessage(statusMessage);
    }
}
