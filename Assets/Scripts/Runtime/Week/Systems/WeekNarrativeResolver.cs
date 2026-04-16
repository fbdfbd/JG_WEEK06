using System;
using System.Collections.Generic;
using System.Linq;

public static class WeekNarrativeResolver
{
    public static WeekFixedEventPresentation ResolveFixedEvent(
        SO_WeekDefinition weekDefinition,
        RuntimeChildState childState,
        WeekUiTextProvider weekUiText)
    {
        WeekFixedEventData fixedEventDefinition = weekDefinition != null ? weekDefinition.FixedEvent : null;
        if (fixedEventDefinition == null || fixedEventDefinition.Reactions == null || fixedEventDefinition.Reactions.Length == 0)
        {
            return WeekFixedEventPresentation.Empty;
        }

        EChildStatusType dominantStat = GetDominantStat(childState);
        WeekEventReactionData matchedReaction = fixedEventDefinition.Reactions
            .FirstOrDefault(reaction => reaction != null && reaction.DominantStat == dominantStat);
        WeekEventReactionData fallbackReaction = fixedEventDefinition.Reactions
            .FirstOrDefault(reaction => reaction != null);
        WeekEventReactionData selectedReaction = matchedReaction ?? fallbackReaction;

        if (selectedReaction == null)
        {
            return WeekFixedEventPresentation.Empty;
        }

        return new WeekFixedEventPresentation(
            string.IsNullOrWhiteSpace(fixedEventDefinition.Title) ? weekUiText.GetFixedEventTitleFallback() : fixedEventDefinition.Title,
            fixedEventDefinition.SituationText,
            selectedReaction.ReactionText,
            BuildEffectSummaryLine(weekUiText, selectedReaction.Interactions, selectedReaction.HasLegacyStatDeltas),
            dominantStat,
            selectedReaction.Interactions ?? Array.Empty<SO_CardInteractionDefinition>());
    }

    public static WeekPrivateDialoguePresentation ResolvePrivateDialogue(
        SO_WeekDefinition weekDefinition,
        WeekUiTextProvider weekUiText)
    {
        WeekPrivateDialogueData privateDialogueDefinition = weekDefinition != null ? weekDefinition.PrivateDialogue : null;
        if (privateDialogueDefinition == null || privateDialogueDefinition.Choices == null || privateDialogueDefinition.Choices.Length == 0)
        {
            return WeekPrivateDialoguePresentation.Empty;
        }

        WeekDialogueChoicePresentation[] choicePresentations = privateDialogueDefinition.Choices
            .Where(choice => choice != null)
            .Select(choice => new WeekDialogueChoicePresentation(
                choice.Label,
                choice.ResponseLine,
                BuildEffectSummaryLine(weekUiText, choice.Interactions, choice.HasLegacyStatDeltas),
                choice.Interactions ?? Array.Empty<SO_CardInteractionDefinition>()))
            .ToArray();

        return new WeekPrivateDialoguePresentation(
            string.IsNullOrWhiteSpace(privateDialogueDefinition.Title) ? weekUiText.GetPrivateDialogueTitleFallback() : privateDialogueDefinition.Title,
            privateDialogueDefinition.OpeningLine,
            choicePresentations);
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

    private static string BuildEffectSummaryLine(
        WeekUiTextProvider weekUiText,
        IReadOnlyList<SO_CardInteractionDefinition> interactions,
        bool hasLegacyStatDeltas)
    {
        List<string> effectNames = interactions?
            .Where(interaction => interaction != null)
            .Select(GetEffectDisplayName)
            .Where(effectName => !string.IsNullOrWhiteSpace(effectName))
            .ToList()
            ?? new List<string>();

        if (effectNames.Count > 0)
        {
            return string.Join(" / ", effectNames);
        }

        if (hasLegacyStatDeltas)
        {
            return weekUiText.GetLegacyNarrativeEffectMessage();
        }

        return weekUiText.GetNoEffectSummary();
    }

    private static string GetEffectDisplayName(SO_CardInteractionDefinition interaction)
    {
        if (interaction == null)
        {
            return string.Empty;
        }

        return string.IsNullOrWhiteSpace(interaction.name)
            ? interaction.GetType().Name
            : interaction.name;
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
        Array.Empty<SO_CardInteractionDefinition>());

    public WeekFixedEventPresentation(
        string title,
        string situationText,
        string reactionText,
        string effectSummaryLine,
        EChildStatusType dominantStat,
        IReadOnlyList<SO_CardInteractionDefinition> interactions)
    {
        Title = title;
        SituationText = situationText;
        ReactionText = reactionText;
        EffectSummaryLine = effectSummaryLine;
        DominantStat = dominantStat;
        Interactions = interactions;
    }

    public string Title { get; }
    public string SituationText { get; }
    public string ReactionText { get; }
    public string EffectSummaryLine { get; }
    public EChildStatusType DominantStat { get; }
    public IReadOnlyList<SO_CardInteractionDefinition> Interactions { get; }
    public bool HasContent => !string.IsNullOrWhiteSpace(Title);
}

public readonly struct WeekPrivateDialoguePresentation
{
    public static WeekPrivateDialoguePresentation Empty => new(
        string.Empty,
        string.Empty,
        Array.Empty<WeekDialogueChoicePresentation>());

    public WeekPrivateDialoguePresentation(
        string title,
        string openingLine,
        IReadOnlyList<WeekDialogueChoicePresentation> choices)
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
        string effectSummaryLine,
        IReadOnlyList<SO_CardInteractionDefinition> interactions)
    {
        Label = label;
        ResponseLine = responseLine;
        EffectSummaryLine = effectSummaryLine;
        Interactions = interactions;
    }

    public string Label { get; }
    public string ResponseLine { get; }
    public string EffectSummaryLine { get; }
    public IReadOnlyList<SO_CardInteractionDefinition> Interactions { get; }
}
