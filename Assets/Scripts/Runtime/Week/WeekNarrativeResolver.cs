using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class WeekNarrativeResolver
{
    public static WeekFixedEventPresentation ResolveFixedEvent(SO_WeekDefinition weekDefinition, RuntimeChildState childState)
    {
        WeekFixedEventData fixedEvent = weekDefinition != null ? weekDefinition.FixedEvent : null;
        if (fixedEvent == null || fixedEvent.Reactions == null || fixedEvent.Reactions.Length == 0)
        {
            return WeekFixedEventPresentation.Empty;
        }

        EChildStatusType dominantStat = GetDominantStat(childState);
        WeekEventReactionData selectedReaction =
            fixedEvent.Reactions.FirstOrDefault(reaction => reaction != null && reaction.DominantStat == dominantStat)
            ?? fixedEvent.Reactions.FirstOrDefault(reaction => reaction != null);

        if (selectedReaction == null)
        {
            return WeekFixedEventPresentation.Empty;
        }

        return new WeekFixedEventPresentation(
            string.IsNullOrWhiteSpace(fixedEvent.Title) ? "주차 확정 사건" : fixedEvent.Title,
            fixedEvent.SituationText,
            selectedReaction.ReactionText,
            BuildDeltaLine(selectedReaction.StatDeltas),
            dominantStat,
            selectedReaction.StatDeltas ?? Array.Empty<StatusDeltaData>());
    }

    public static WeekPrivateDialoguePresentation ResolvePrivateDialogue(SO_WeekDefinition weekDefinition)
    {
        WeekPrivateDialogueData privateDialogue = weekDefinition != null ? weekDefinition.PrivateDialogue : null;
        if (privateDialogue == null || privateDialogue.Choices == null || privateDialogue.Choices.Length == 0)
        {
            return WeekPrivateDialoguePresentation.Empty;
        }

        return new WeekPrivateDialoguePresentation(
            string.IsNullOrWhiteSpace(privateDialogue.Title) ? "개인 대화" : privateDialogue.Title,
            privateDialogue.OpeningLine,
            privateDialogue.Choices
                .Where(choice => choice != null)
                .Select(choice => new WeekDialogueChoicePresentation(choice.Label, choice.ResponseLine, BuildDeltaLine(choice.StatDeltas), choice))
                .ToArray());
    }

    public static void ApplyDeltas(RuntimeChildState childState, IReadOnlyList<StatusDeltaData> deltas)
    {
        if (childState == null || deltas == null)
        {
            return;
        }

        foreach (StatusDeltaData delta in deltas)
        {
            if (delta == null || delta.Amount == 0)
            {
                continue;
            }

            childState.AddStat(delta.StatType, delta.Amount);
        }
    }

    public static ENemoVisualState GetVisualStateForStat(EChildStatusType dominantStat)
    {
        return dominantStat switch
        {
            EChildStatusType.Trust => ENemoVisualState.Trusting,
            EChildStatusType.Curiosity => ENemoVisualState.Curious,
            EChildStatusType.Anxiety => ENemoVisualState.Anxious,
            EChildStatusType.Obedience => ENemoVisualState.Obedient,
            _ => ENemoVisualState.Neutral,
        };
    }

    public static EChildStatusType GetDominantStat(RuntimeChildState childState)
    {
        return Enum.GetValues(typeof(EChildStatusType))
            .Cast<EChildStatusType>()
            .OrderByDescending(childState.GetStat)
            .First();
    }

    private static string BuildDeltaLine(IReadOnlyList<StatusDeltaData> deltas)
    {
        if (deltas == null || deltas.Count == 0)
        {
            return "스탯 변화 없음";
        }

        StringBuilder builder = new();
        bool first = true;

        foreach (StatusDeltaData delta in deltas)
        {
            if (delta == null || delta.Amount == 0)
            {
                continue;
            }

            if (!first)
            {
                builder.Append("  ·  ");
            }

            builder.Append(GetStatLabel(delta.StatType));
            builder.Append(delta.Amount > 0 ? " +" : " ");
            builder.Append(delta.Amount);
            first = false;
        }

        return first ? "스탯 변화 없음" : builder.ToString();
    }

    private static string GetStatLabel(EChildStatusType statType)
    {
        return statType switch
        {
            EChildStatusType.Trust => "신뢰",
            EChildStatusType.Curiosity => "호기심",
            EChildStatusType.Anxiety => "불안",
            EChildStatusType.Obedience => "순응",
            _ => "상태",
        };
    }
}

public readonly struct WeekFixedEventPresentation
{
    public static WeekFixedEventPresentation Empty => new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        EChildStatusType.Trust,
        Array.Empty<StatusDeltaData>());

    public WeekFixedEventPresentation(
        string title,
        string situationText,
        string reactionText,
        string statDeltaLine,
        EChildStatusType dominantStat,
        IReadOnlyList<StatusDeltaData> statDeltas)
    {
        Title = title;
        SituationText = situationText;
        ReactionText = reactionText;
        StatDeltaLine = statDeltaLine;
        DominantStat = dominantStat;
        StatDeltas = statDeltas;
    }

    public string Title { get; }
    public string SituationText { get; }
    public string ReactionText { get; }
    public string StatDeltaLine { get; }
    public EChildStatusType DominantStat { get; }
    public IReadOnlyList<StatusDeltaData> StatDeltas { get; }
    public bool HasContent => !string.IsNullOrWhiteSpace(Title);
}

public readonly struct WeekPrivateDialoguePresentation
{
    public static WeekPrivateDialoguePresentation Empty => new(string.Empty, string.Empty, Array.Empty<WeekDialogueChoicePresentation>());

    public WeekPrivateDialoguePresentation(string title, string openingLine, IReadOnlyList<WeekDialogueChoicePresentation> choices)
    {
        Title = title;
        OpeningLine = openingLine;
        Choices = choices;
    }

    public string Title { get; }
    public string OpeningLine { get; }
    public IReadOnlyList<WeekDialogueChoicePresentation> Choices { get; }
    public bool HasContent => !string.IsNullOrWhiteSpace(Title) && Choices != null && Choices.Count > 0;
}

public readonly struct WeekDialogueChoicePresentation
{
    public WeekDialogueChoicePresentation(
        string label,
        string responseLine,
        string statDeltaLine,
        DialogueChoiceData sourceData)
    {
        Label = label;
        ResponseLine = responseLine;
        StatDeltaLine = statDeltaLine;
        SourceData = sourceData;
    }

    public string Label { get; }
    public string ResponseLine { get; }
    public string StatDeltaLine { get; }
    public DialogueChoiceData SourceData { get; }
}
