using System;
using System.Collections.Generic;

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
    private readonly List<DialogueLogEntry> _entries = new();

    public event Action Changed;

    public IReadOnlyList<DialogueLogEntry> Entries => _entries;

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
