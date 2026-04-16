using System.Linq;

public sealed class WeekFlowPresenter
{
    private readonly WeekFlowViewBase _view;
    private readonly WeekFlowRuntimeState _runtimeState;
    private readonly WeekUiTextProvider _weekUiText;
    private readonly WeekSelectionState _weekSelectionState;
    private readonly WeekSequenceState _weekSequenceState;

    public WeekFlowPresenter(
        WeekFlowViewBase view,
        WeekFlowRuntimeState runtimeState,
        WeekUiTextProvider weekUiText,
        WeekSelectionState weekSelectionState,
        WeekSequenceState weekSequenceState)
    {
        _view = view;
        _runtimeState = runtimeState;
        _weekUiText = weekUiText;
        _weekSelectionState = weekSelectionState;
        _weekSequenceState = weekSequenceState;
    }

    public void RefreshAll()
    {
        PublishWeekHeader();
        PublishSelectionEntries();
        PublishChildState();
        PublishStatusMessage();
        PublishCurrentNemoFeedback();
    }

    public void PublishSelectionEntries()
    {
        WeekSelectionEntryPresentation[] selectionPresentations = _weekSelectionState.BuildSelectionPresentations(
            WeekFlowQueryUtility.GetCurrentWeekEntries(_weekSequenceState.CurrentWeekDefinition),
            _weekUiText.GetUnknownCardType());
        _view?.RenderSelections(selectionPresentations);
    }

    public void PublishChildState()
    {
        WeekStatPresentation[] statPresentations =
        {
            new(EChildStatusType.Trust, _weekUiText.GetStatLabel(EChildStatusType.Trust), _runtimeState.ChildState.GetStat(EChildStatusType.Trust)),
            new(EChildStatusType.Curiosity, _weekUiText.GetStatLabel(EChildStatusType.Curiosity), _runtimeState.ChildState.GetStat(EChildStatusType.Curiosity)),
            new(EChildStatusType.Anxiety, _weekUiText.GetStatLabel(EChildStatusType.Anxiety), _runtimeState.ChildState.GetStat(EChildStatusType.Anxiety)),
            new(EChildStatusType.Obedience, _weekUiText.GetStatLabel(EChildStatusType.Obedience), _runtimeState.ChildState.GetStat(EChildStatusType.Obedience)),
        };

        string[] flagNames = _runtimeState.ChildState.Flags
            .Select(flagType => flagType.ToString())
            .ToArray();

        string[] reactionLogs = _runtimeState.ChildState.ReactionLogs
            .ToArray();

        _view?.RenderChildState(new ChildStatePresentation(statPresentations, flagNames, reactionLogs));
    }

    public void PublishStatusMessage()
    {
        _view?.RenderStatusMessage(_runtimeState.StatusMessage);
    }

    public void PublishCurrentNemoFeedback()
    {
        RuntimeResolvedCardRecord lastResolvedCard = _runtimeState.LastWeekResult != null && _runtimeState.LastWeekResult.ResolvedCards.Count > 0
            ? _runtimeState.LastWeekResult.ResolvedCards[_runtimeState.LastWeekResult.ResolvedCards.Count - 1]
            : null;

        PublishNemoFeedback(lastResolvedCard == null
            ? new NemoFeedbackPresentation(ENemoVisualState.Neutral, _weekUiText.GetDefaultNemoLine())
            : NemoFeedbackResolver.Resolve(_runtimeState.ChildState, lastResolvedCard));
    }

    public void PublishDefaultNemoFeedback()
    {
        PublishNemoFeedback(new NemoFeedbackPresentation(
            ENemoVisualState.Neutral,
            _weekUiText.GetDefaultNemoLine()));
    }

    public void PublishNemoFeedback(NemoFeedbackPresentation presentation)
    {
        _view?.PresentNemoFeedback(presentation);
    }

    public void ShowWeekFeedback(WeekFeedbackPresentation presentation)
    {
        _view?.ShowWeekFeedback(presentation);
    }

    public void ShowWeekEvent(WeekFixedEventPresentation presentation)
    {
        _view?.ShowWeekEvent(presentation);
    }

    public void ShowPrivateDialogue(WeekPrivateDialoguePresentation presentation)
    {
        _view?.ShowPrivateDialogue(presentation);
    }

    public void ShowPrivateDialogueResult(WeekDialogueChoiceResultPresentation presentation)
    {
        _view?.ShowPrivateDialogueResult(presentation);
    }

    public void ShowEnding(EndingPresentation presentation)
    {
        _view?.ShowEnding(presentation);
    }

    private void PublishWeekHeader()
    {
        SO_WeekDefinition currentWeekDefinition = _weekSequenceState.CurrentWeekDefinition;
        if (currentWeekDefinition == null)
        {
            _view?.RenderWeekHeader(new WeekHeaderPresentation(
                _weekUiText.GetNoWeekLabel(),
                _weekUiText.GetNoWeekTitle(),
                string.Empty));
            return;
        }

        _view?.RenderWeekHeader(new WeekHeaderPresentation(
            _weekUiText.GetWeekLabel(currentWeekDefinition.WeekIndex),
            currentWeekDefinition.Title,
            currentWeekDefinition.Summary));
    }
}
