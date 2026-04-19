using System;
using System.Collections.Generic;
using System.Linq;

public static class WeekNarrativeResolver
{
    public static SO_InteractiveEventDefinition[] ResolvePendingEvents(
        SO_WeekDefinition weekDefinition,
        RuntimeChildState childState,
        RuntimeInformationControlResult informationControlResult)
    {
        List<SO_InteractiveEventDefinition> pendingEvents = new();
        pendingEvents.AddRange(ResolveRoutineEvents(weekDefinition?.DayFlow, childState, informationControlResult));
        pendingEvents.AddRange(ResolveStoryEvents(weekDefinition?.DayFlow, childState, informationControlResult));
        pendingEvents.AddRange(ResolveNightDialogues(weekDefinition?.NightFlow, childState, informationControlResult));
        return pendingEvents.ToArray();
    }

    public static InteractiveEventPresentation CreatePresentation(
        RuntimeInteractiveEventSession eventSession,
        RuntimeChildState childState,
        WeekUiTextProvider weekUiText)
    {
        if (eventSession?.EventDefinition == null || eventSession.CurrentStep == null)
        {
            return InteractiveEventPresentation.Empty;
        }

        SO_InteractiveEventStepDefinition step = eventSession.CurrentStep;
        InteractiveEventChoicePresentation[] choices = step.Choices?
            .Where(choice => choice != null)
            .Select(choice => new InteractiveEventChoicePresentation(
                choice.Label,
                BuildEffectSummary(choice.Interactions, weekUiText)))
            .ToArray()
            ?? Array.Empty<InteractiveEventChoicePresentation>();

        DialogueLinePresentation[] dialogueLines = BuildDialogueLines(
            step.DialogueLines,
            step.NemoLine);

        return new InteractiveEventPresentation(
            string.IsNullOrWhiteSpace(step.TitleOverride) ? eventSession.EventDefinition.Title : step.TitleOverride,
            step.BodyText,
            BuildEffectSummary(step.OnEnterInteractions, weekUiText),
            ResolveVisualState(step, childState),
            dialogueLines,
            choices,
            choices.Length == 0);
    }

    public static InteractiveEventChoiceResultPresentation CreateChoiceResultPresentation(
        InteractiveEventChoiceData selectedChoice,
        WeekUiTextProvider weekUiText)
    {
        return new InteractiveEventChoiceResultPresentation(
            BuildDialogueLines(
                selectedChoice?.ResponseDialogueLines,
                selectedChoice?.ResponseLine),
            BuildEffectSummary(selectedChoice?.Interactions, weekUiText));
    }

    public static DialogueLinePresentation GetPrimaryDialogueLine(
        IReadOnlyList<DialogueLinePresentation> dialogueLines,
        string fallbackSpeakerName = NemoFeedbackResolver.DefaultSpeakerName)
    {
        if (dialogueLines != null)
        {
            foreach (DialogueLinePresentation dialogueLine in dialogueLines)
            {
                if (!dialogueLine.HasContent)
                {
                    continue;
                }

                string speakerName = string.IsNullOrWhiteSpace(dialogueLine.SpeakerName)
                    ? fallbackSpeakerName
                    : dialogueLine.SpeakerName;
                return new DialogueLinePresentation(speakerName, dialogueLine.Text);
            }
        }

        return new DialogueLinePresentation(fallbackSpeakerName, string.Empty);
    }

    public static ENemoVisualState GetVisualStateForCurrentState(RuntimeChildState childState)
    {
        return GetVisualStateForStat(GetDominantStat(childState));
    }

    public static ENemoVisualState GetVisualStateForStat(EChildStatusType dominantStat)
    {
        return dominantStat switch
        {
            EChildStatusType.Trust => ENemoVisualState.Trusting,
            EChildStatusType.Affinity => ENemoVisualState.Trusting,
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

    private static IEnumerable<SO_InteractiveEventDefinition> ResolveRoutineEvents(
        SO_WeekDayFlowDefinition dayFlow,
        RuntimeChildState childState,
        RuntimeInformationControlResult informationControlResult)
    {
        return dayFlow?.RoutineEvents?
            .Where(routineEvent => routineEvent != null)
            .Select(routineEvent => new
            {
                Event = routineEvent,
                Score = ResolveRoutineMatchScore(routineEvent, informationControlResult),
            })
            .Where(item => item.Score > 0 && IsEventAvailable(item.Event, childState, informationControlResult))
            .OrderByDescending(item => item.Score)
            .ThenByDescending(item => item.Event.Priority)
            .Select(item => item.Event)
            ?? Enumerable.Empty<SO_InteractiveEventDefinition>();
    }

    private static IEnumerable<SO_InteractiveEventDefinition> ResolveStoryEvents(
        SO_WeekDayFlowDefinition dayFlow,
        RuntimeChildState childState,
        RuntimeInformationControlResult informationControlResult)
    {
        return ResolveEligibleEvents(dayFlow?.StoryEvents, childState, informationControlResult);
    }

    private static IEnumerable<SO_InteractiveEventDefinition> ResolveNightDialogues(
        SO_WeekNightFlowDefinition nightFlow,
        RuntimeChildState childState,
        RuntimeInformationControlResult informationControlResult)
    {
        return ResolveEligibleEvents(nightFlow?.Dialogues, childState, informationControlResult).Take(1);
    }

    private static IEnumerable<SO_InteractiveEventDefinition> ResolveEligibleEvents(
        IEnumerable<SO_InteractiveEventDefinition> events,
        RuntimeChildState childState,
        RuntimeInformationControlResult informationControlResult)
    {
        return events?
            .Where(eventDefinition => IsEventAvailable(eventDefinition, childState, informationControlResult))
            .OrderByDescending(eventDefinition => eventDefinition.Priority)
            ?? Enumerable.Empty<SO_InteractiveEventDefinition>();
    }

    private static bool IsEventAvailable(
        SO_InteractiveEventDefinition eventDefinition,
        RuntimeChildState childState,
        RuntimeInformationControlResult informationControlResult)
    {
        if (eventDefinition?.FirstStep == null)
        {
            return false;
        }

        WeekEventConditionData conditions = eventDefinition.Conditions;
        return HasRequiredFlags(childState, conditions) &&
               HasNoBlockedFlags(childState, conditions) &&
               MeetsStatRequirements(childState, conditions) &&
               MeetsInformationRequirements(informationControlResult, conditions);
    }

    private static bool HasRequiredFlags(RuntimeChildState childState, WeekEventConditionData conditions)
    {
        return conditions?.RequiredFlags == null || conditions.RequiredFlags.All(childState.HasFlag);
    }

    private static bool HasNoBlockedFlags(RuntimeChildState childState, WeekEventConditionData conditions)
    {
        return conditions?.BlockedFlags == null || conditions.BlockedFlags.All(flagType => !childState.HasFlag(flagType));
    }

    private static bool MeetsStatRequirements(RuntimeChildState childState, WeekEventConditionData conditions)
    {
        if (conditions?.StatRequirements == null)
        {
            return true;
        }

        return conditions.StatRequirements.All(requirement =>
        {
            int value = childState.GetStat(requirement.StatType);
            bool meetsMinimum = !requirement.UseMinimum || value >= requirement.MinimumValue;
            bool meetsMaximum = !requirement.UseMaximum || value <= requirement.MaximumValue;
            return meetsMinimum && meetsMaximum;
        });
    }

    private static bool MeetsInformationRequirements(
        RuntimeInformationControlResult informationControlResult,
        WeekEventConditionData conditions)
    {
        if (conditions?.InformationRequirements == null)
        {
            return true;
        }

        return conditions.InformationRequirements.All(requirement =>
        {
            ECardOptionSemantic? semanticFilter = requirement.UseSemanticFilter ? requirement.Semantic : null;
            int count = informationControlResult.CountSelectionsForType(requirement.InformationType, semanticFilter);
            return count >= requirement.MinimumCount;
        });
    }

    private static int ResolveRoutineMatchScore(
        SO_DayRoutineEventDefinition routineEvent,
        RuntimeInformationControlResult informationControlResult)
    {
        if (routineEvent?.RelatedInformationTypes == null || routineEvent.RelatedInformationTypes.Length == 0)
        {
            return 0;
        }

        if (routineEvent.PreferredSemantics == null || routineEvent.PreferredSemantics.Length == 0)
        {
            return informationControlResult.CountSelectionsForAnyType(routineEvent.RelatedInformationTypes);
        }

        return routineEvent.PreferredSemantics.Sum(semantic =>
            informationControlResult.CountSelectionsForAnyType(routineEvent.RelatedInformationTypes, semantic));
    }

    private static ENemoVisualState ResolveVisualState(
        SO_InteractiveEventStepDefinition step,
        RuntimeChildState childState)
    {
        return step.UseCustomVisualState
            ? step.VisualState
            : GetVisualStateForCurrentState(childState);
    }

    private static string BuildEffectSummary(
        IReadOnlyList<SO_CardInteractionDefinition> interactions,
        WeekUiTextProvider weekUiText)
    {
        string[] effectNames = interactions?
            .Where(interaction => interaction != null)
            .Select(interaction => interaction.name)
            .Where(effectName => !string.IsNullOrWhiteSpace(effectName))
            .ToArray()
            ?? Array.Empty<string>();

        return effectNames.Length == 0
            ? weekUiText.GetNoEffectSummary()
            : string.Join(" / ", effectNames);
    }

    private static DialogueLinePresentation[] BuildDialogueLines(
        IReadOnlyList<DialogueLineData> dialogueLines,
        string legacyFallbackLine)
    {
        DialogueLinePresentation[] lines = dialogueLines?
            .Where(dialogueLine => dialogueLine != null && !string.IsNullOrWhiteSpace(dialogueLine.Text))
            .Select(dialogueLine => new DialogueLinePresentation(
                ResolveSpeakerName(dialogueLine.Speaker),
                dialogueLine.Text))
            .ToArray()
            ?? Array.Empty<DialogueLinePresentation>();

        if (lines.Length > 0)
        {
            return lines;
        }

        return string.IsNullOrWhiteSpace(legacyFallbackLine)
            ? Array.Empty<DialogueLinePresentation>()
            : new[] { new DialogueLinePresentation(NemoFeedbackResolver.DefaultSpeakerName, legacyFallbackLine) };
    }

    private static string ResolveSpeakerName(SO_DialogueSpeakerDefinition speaker)
    {
        if (speaker == null)
        {
            return NemoFeedbackResolver.DefaultSpeakerName;
        }

        if (!string.IsNullOrWhiteSpace(speaker.DisplayName))
        {
            return speaker.DisplayName;
        }

        if (!string.IsNullOrWhiteSpace(speaker.name))
        {
            return speaker.name;
        }

        return NemoFeedbackResolver.DefaultSpeakerName;
    }
}

public readonly struct InteractiveEventPresentation
{
    public static InteractiveEventPresentation Empty => new(
        string.Empty,
        string.Empty,
        string.Empty,
        ENemoVisualState.Neutral,
        Array.Empty<DialogueLinePresentation>(),
        Array.Empty<InteractiveEventChoicePresentation>(),
        false);

    public InteractiveEventPresentation(
        string title,
        string bodyText,
        string effectSummaryLine,
        ENemoVisualState visualState,
        IReadOnlyList<DialogueLinePresentation> dialogueLines,
        IReadOnlyList<InteractiveEventChoicePresentation> choices,
        bool canContinue)
    {
        Title = title;
        BodyText = bodyText;
        EffectSummaryLine = effectSummaryLine;
        VisualState = visualState;
        DialogueLines = dialogueLines;
        Choices = choices;
        CanContinue = canContinue;
    }

    public string Title { get; }
    public string BodyText { get; }
    public string EffectSummaryLine { get; }
    public ENemoVisualState VisualState { get; }
    public IReadOnlyList<DialogueLinePresentation> DialogueLines { get; }
    public IReadOnlyList<InteractiveEventChoicePresentation> Choices { get; }
    public bool CanContinue { get; }
    public bool HasContent => !string.IsNullOrWhiteSpace(Title) || !string.IsNullOrWhiteSpace(BodyText);
}

public readonly struct InteractiveEventChoicePresentation
{
    public InteractiveEventChoicePresentation(
        string label,
        string effectSummaryLine)
    {
        Label = label;
        EffectSummaryLine = effectSummaryLine;
    }

    public string Label { get; }
    public string EffectSummaryLine { get; }
}
