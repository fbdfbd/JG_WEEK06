public sealed class WeekFlowNarrativeHandler
{
    private readonly WeekFlowRuntimeState _runtimeState;
    private readonly WeekFlowPresenter _presenter;
    private readonly WeekUiTextProvider _weekUiText;
    private readonly WeekSelectionState _weekSelectionState;
    private readonly WeekSequenceState _weekSequenceState;

    public WeekFlowNarrativeHandler(
        WeekFlowRuntimeState runtimeState,
        WeekFlowPresenter presenter,
        WeekUiTextProvider weekUiText,
        WeekSelectionState weekSelectionState,
        WeekSequenceState weekSequenceState)
    {
        _runtimeState = runtimeState;
        _presenter = presenter;
        _weekUiText = weekUiText;
        _weekSelectionState = weekSelectionState;
        _weekSequenceState = weekSequenceState;
    }

    public void CloseWeekFeedback()
    {
        ContinuePostWeekFlow();
    }

    public void ContinueWeekEvent()
    {
        if (!_runtimeState.PendingWeekEvent.HasContent || _runtimeState.HasAppliedPendingWeekEvent)
        {
            ContinuePostWeekFlow();
            return;
        }

        GameplayInteractionExecutor.ApplyAll(_runtimeState.PendingWeekEvent.Interactions, _runtimeState.ChildState);
        _runtimeState.HasAppliedPendingWeekEvent = true;

        _presenter.PublishChildState();
        _presenter.PublishNemoFeedback(new NemoFeedbackPresentation(
            WeekNarrativeResolver.GetVisualStateForStat(_runtimeState.PendingWeekEvent.DominantStat),
            _runtimeState.PendingWeekEvent.ReactionText));

        PublishStatusMessage(_weekUiText.GetWeekEventAppliedMessage());
        ContinuePostWeekFlow();
    }

    public void SelectPrivateDialogueChoice(int choiceIndex)
    {
        if (_runtimeState.HasSelectedDialogueChoice || !_runtimeState.PendingPrivateDialogue.HasContent)
        {
            return;
        }

        if (choiceIndex < 0 || choiceIndex >= _runtimeState.PendingPrivateDialogue.Choices.Count)
        {
            return;
        }

        _runtimeState.SelectedDialogueChoice = _runtimeState.PendingPrivateDialogue.Choices[choiceIndex];
        _runtimeState.HasSelectedDialogueChoice = true;

        GameplayInteractionExecutor.ApplyAll(_runtimeState.SelectedDialogueChoice.Interactions, _runtimeState.ChildState);

        _presenter.PublishChildState();
        _presenter.PublishNemoFeedback(new NemoFeedbackPresentation(
            WeekNarrativeResolver.GetVisualStateForStat(WeekNarrativeResolver.GetDominantStat(_runtimeState.ChildState)),
            _runtimeState.SelectedDialogueChoice.ResponseLine));

        _presenter.ShowPrivateDialogueResult(new WeekDialogueChoiceResultPresentation(
            _runtimeState.SelectedDialogueChoice.ResponseLine,
            _runtimeState.SelectedDialogueChoice.EffectSummaryLine));

        PublishStatusMessage(_weekUiText.GetPrivateDialogueChoiceAppliedMessage());
    }

    public void ContinuePrivateDialogue()
    {
        ContinuePostWeekFlow();
    }

    private void ContinuePostWeekFlow()
    {
        if (_runtimeState.PendingWeekEvent.HasContent && !_runtimeState.HasAppliedPendingWeekEvent)
        {
            _presenter.ShowWeekEvent(_runtimeState.PendingWeekEvent);
            return;
        }

        if (_runtimeState.PendingPrivateDialogue.HasContent && !_runtimeState.HasSelectedDialogueChoice)
        {
            _presenter.ShowPrivateDialogue(_runtimeState.PendingPrivateDialogue);
            return;
        }

        if (_runtimeState.ShouldShowEndingAfterNarrative)
        {
            ShowEnding();
            return;
        }

        if (_runtimeState.ShouldAdvanceToNextWeekAfterNarrative)
        {
            MoveToNextWeek();
            return;
        }

        _runtimeState.ClearPendingNarrativeState();
    }

    private void ShowEnding()
    {
        _runtimeState.ShouldShowEndingAfterNarrative = false;
        _runtimeState.HasReachedEnding = true;

        _presenter.ShowEnding(EndingResolver.Resolve(_runtimeState.ChildState));
        PublishStatusMessage(_weekUiText.GetEndingReachedMessage());
    }

    private void MoveToNextWeek()
    {
        _runtimeState.ShouldAdvanceToNextWeekAfterNarrative = false;
        if (!_weekSequenceState.TryMoveToNextWeek())
        {
            _runtimeState.ClearPendingNarrativeState();
            return;
        }

        WeekCardEntryData[] currentWeekEntries = WeekFlowQueryUtility.GetCurrentWeekEntries(_weekSequenceState.CurrentWeekDefinition);
        _weekSelectionState.ApplyWeekEntries(currentWeekEntries);
        _weekSelectionState.ResetAllSelections(currentWeekEntries);
        _runtimeState.LastWeekResult = null;
        _runtimeState.ClearPendingNarrativeState();

        SO_WeekDefinition currentWeekDefinition = _weekSequenceState.CurrentWeekDefinition;
        PublishStatusMessage(currentWeekDefinition == null
            ? _weekUiText.GetMovedToNextWeekFallbackMessage()
            : _weekUiText.GetReadyForWeekMessage(currentWeekDefinition.WeekIndex));

        _presenter.RefreshAll();
        _presenter.PublishDefaultNemoFeedback();
    }

    private void PublishStatusMessage(string statusMessage)
    {
        _runtimeState.SetStatusMessage(statusMessage);
        _presenter.PublishStatusMessage();
    }
}
