public enum EWeekFlowTransitionPhase
{
    Enter,
    Exit
}

public sealed class WeekFlowTransitionContext
{
    public WeekFlowTransitionContext(
        EWeekFlowTransitionPhase phase,
        SO_WeekFlowCueDefinition cue,
        WeekFlowScreen screen = null,
        SO_WeekDefinition weekDefinition = null)
    {
        Phase = phase;
        Cue = cue;
        Screen = screen;
        WeekDefinition = weekDefinition;
    }

    public EWeekFlowTransitionPhase Phase { get; }
    public SO_WeekFlowCueDefinition Cue { get; }
    public WeekFlowScreen Screen { get; }
    public SO_WeekDefinition WeekDefinition { get; }
    public bool IsWeekChange => WeekDefinition != null && Screen == null;
}

public enum EDialogueLogSource
{
    WeekFeedback,
    EventStep,
    ChoiceResult,
    Ending
}

public readonly struct DialogueLogEntry
{
    public DialogueLogEntry(
        EDialogueLogSource source,
        string title,
        string speakerName,
        string text)
    {
        Source = source;
        Title = title;
        SpeakerName = speakerName;
        Text = text;
    }

    public EDialogueLogSource Source { get; }
    public string Title { get; }
    public string SpeakerName { get; }
    public string Text { get; }
}

public sealed class WeekFlowDialogueLogService
{
    private readonly System.Collections.Generic.List<DialogueLogEntry> _entries = new();

    public event System.Action Changed;

    public System.Collections.Generic.IReadOnlyList<DialogueLogEntry> Entries => _entries;

    public void Clear()
    {
        _entries.Clear();
        Changed?.Invoke();
    }

    public void Append(DialogueLogEntry entry)
    {
        if (string.IsNullOrWhiteSpace(entry.Text))
        {
            return;
        }

        _entries.Add(entry);
        Changed?.Invoke();
    }
}
