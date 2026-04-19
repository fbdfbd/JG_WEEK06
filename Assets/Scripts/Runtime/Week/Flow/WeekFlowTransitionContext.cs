using System;
using System.Collections;
using UnityEngine;

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

public enum EWeekFlowCutsceneMoment
{
    ScreenEnter,
    LineEnter
}

public readonly struct WeekFlowCutsceneRequest
{
    public WeekFlowCutsceneRequest(
        EWeekFlowCutsceneMoment moment,
        EWeekFlowScreenType screenType,
        string weekId,
        int weekIndex,
        string eventId,
        string stepName,
        int dialogueIndex,
        string choiceLabel,
        RuntimeChildState childState,
        RuntimeWeekResult lastWeekResult)
    {
        Moment = moment;
        ScreenType = screenType;
        WeekId = weekId;
        WeekIndex = weekIndex;
        EventId = eventId;
        StepName = stepName;
        DialogueIndex = dialogueIndex;
        ChoiceLabel = choiceLabel;
        ChildState = childState;
        LastWeekResult = lastWeekResult;
    }

    public EWeekFlowCutsceneMoment Moment { get; }
    public EWeekFlowScreenType ScreenType { get; }
    public string WeekId { get; }
    public int WeekIndex { get; }
    public string EventId { get; }
    public string StepName { get; }
    public int DialogueIndex { get; }
    public string ChoiceLabel { get; }
    public RuntimeChildState ChildState { get; }
    public RuntimeWeekResult LastWeekResult { get; }

    public static WeekFlowCutsceneRequest CreateScreenEnter(
        WeekFlowScreen screen,
        RuntimeChildState childState,
        RuntimeWeekResult lastWeekResult)
    {
        if (screen == null)
        {
            return default;
        }

        return new WeekFlowCutsceneRequest(
            EWeekFlowCutsceneMoment.ScreenEnter,
            screen.ScreenType,
            ResolveWeekId(screen.WeekDefinition),
            screen.WeekDefinition != null ? screen.WeekDefinition.WeekIndex : 0,
            ResolveEventId(screen.EventDefinition),
            ResolveStepName(screen.StepDefinition),
            -1,
            ResolveChoiceLabel(screen.ChoiceData),
            childState,
            lastWeekResult);
    }

    public static WeekFlowCutsceneRequest CreateLineEnter(
        WeekFlowScreen screen,
        int dialogueIndex,
        RuntimeChildState childState,
        RuntimeWeekResult lastWeekResult)
    {
        if (screen == null)
        {
            return default;
        }

        return new WeekFlowCutsceneRequest(
            EWeekFlowCutsceneMoment.LineEnter,
            screen.ScreenType,
            ResolveWeekId(screen.WeekDefinition),
            screen.WeekDefinition != null ? screen.WeekDefinition.WeekIndex : 0,
            ResolveEventId(screen.EventDefinition),
            ResolveStepName(screen.StepDefinition),
            dialogueIndex,
            ResolveChoiceLabel(screen.ChoiceData),
            childState,
            lastWeekResult);
    }

    private static string ResolveWeekId(SO_WeekDefinition weekDefinition)
    {
        if (weekDefinition == null)
        {
            return string.Empty;
        }

        return !string.IsNullOrWhiteSpace(weekDefinition.Id)
            ? weekDefinition.Id
            : weekDefinition.name;
    }

    private static string ResolveEventId(SO_InteractiveEventDefinition eventDefinition)
    {
        if (eventDefinition == null)
        {
            return string.Empty;
        }

        return !string.IsNullOrWhiteSpace(eventDefinition.Id)
            ? eventDefinition.Id
            : eventDefinition.name;
    }

    private static string ResolveStepName(SO_InteractiveEventStepDefinition stepDefinition)
    {
        return stepDefinition != null ? stepDefinition.name : string.Empty;
    }

    private static string ResolveChoiceLabel(InteractiveEventChoiceData choiceData)
    {
        return choiceData != null ? choiceData.Label : string.Empty;
    }
}

public abstract class WeekFlowCutsceneBridgeBase : MonoBehaviour
{
    public abstract bool IsPlaying { get; }
    public abstract bool IsBlocking { get; }

    public abstract IEnumerator Play(WeekFlowCutsceneRequest request);
    public abstract bool TrySkip();
    public abstract void StopImmediate();
}

public sealed class WeekFlowCutsceneResolver
{
    public bool TryResolveCutsceneId(
        WeekFlowCutsceneRequest request,
        SO_WeekFlowCutsceneCatalog catalog,
        out string cutsceneId)
    {
        cutsceneId = string.Empty;
        if (catalog == null || catalog.Entries == null || catalog.Entries.Length == 0)
        {
            return false;
        }

        int bestScore = int.MinValue;

        for (int index = 0; index < catalog.Entries.Length; index++)
        {
            WeekFlowCutsceneEntryData entry = catalog.Entries[index];
            if (!IsMatch(entry, request, out int score))
            {
                continue;
            }

            if (score <= bestScore)
            {
                continue;
            }

            bestScore = score;
            cutsceneId = entry.CutsceneId;
        }

        return !string.IsNullOrWhiteSpace(cutsceneId);
    }

    private static bool IsMatch(WeekFlowCutsceneEntryData entry, WeekFlowCutsceneRequest request, out int score)
    {
        score = 0;

        if (entry == null || entry.Moment != request.Moment || string.IsNullOrWhiteSpace(entry.CutsceneId))
        {
            return false;
        }

        if (entry.UseScreenType)
        {
            if (entry.ScreenType != request.ScreenType)
            {
                return false;
            }

            score++;
        }

        if (!IsStringFieldMatch(entry.WeekId, request.WeekId, ref score))
        {
            return false;
        }

        if (!IsStringFieldMatch(entry.EventId, request.EventId, ref score))
        {
            return false;
        }

        if (!IsStringFieldMatch(entry.StepName, request.StepName, ref score))
        {
            return false;
        }

        if (!IsStringFieldMatch(entry.ChoiceLabel, request.ChoiceLabel, ref score))
        {
            return false;
        }

        if (entry.DialogueIndex >= 0)
        {
            if (entry.DialogueIndex != request.DialogueIndex)
            {
                return false;
            }

            score++;
        }

        return true;
    }

    private static bool IsStringFieldMatch(string expected, string actual, ref int score)
    {
        if (string.IsNullOrWhiteSpace(expected))
        {
            return true;
        }

        if (!string.Equals(expected, actual, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        score++;
        return true;
    }
}
