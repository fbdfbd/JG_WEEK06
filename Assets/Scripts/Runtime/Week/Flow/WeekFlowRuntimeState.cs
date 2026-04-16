public sealed class WeekFlowRuntimeState
{
    public WeekFlowRuntimeState()
    {
        ChildState = new RuntimeChildState();
        ClearPendingNarrativeState();
        StatusMessage = "Week flow is ready.";
    }

    public RuntimeChildState ChildState { get; private set; }
    public RuntimeWeekResult LastWeekResult { get; set; }
    public WeekFixedEventPresentation PendingWeekEvent { get; set; }
    public WeekPrivateDialoguePresentation PendingPrivateDialogue { get; set; }
    public WeekDialogueChoicePresentation SelectedDialogueChoice { get; set; }
    public bool HasAppliedPendingWeekEvent { get; set; }
    public bool HasSelectedDialogueChoice { get; set; }
    public bool ShouldShowEndingAfterNarrative { get; set; }
    public bool ShouldAdvanceToNextWeekAfterNarrative { get; set; }
    public bool HasReachedEnding { get; set; }
    public string StatusMessage { get; private set; }

    public void ResetChildState()
    {
        ChildState = new RuntimeChildState();
        LastWeekResult = null;
        HasReachedEnding = false;
        ClearPendingNarrativeState();
    }

    public void ClearPendingNarrativeState()
    {
        PendingWeekEvent = WeekFixedEventPresentation.Empty;
        PendingPrivateDialogue = WeekPrivateDialoguePresentation.Empty;
        SelectedDialogueChoice = default;
        HasAppliedPendingWeekEvent = false;
        HasSelectedDialogueChoice = false;
        ShouldShowEndingAfterNarrative = false;
        ShouldAdvanceToNextWeekAfterNarrative = false;
    }

    public void SetStatusMessage(string statusMessage)
    {
        StatusMessage = statusMessage;
    }
}
