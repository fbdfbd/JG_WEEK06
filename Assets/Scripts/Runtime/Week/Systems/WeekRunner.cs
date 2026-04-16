using System;
using System.Collections.Generic;
using System.Linq;

public sealed class WeekRunner
{
    public RuntimeWeekResult RunWeek(
        SO_WeekDefinition weekDefinition,
        RuntimeChildState childState,
        IReadOnlyList<RuntimeWeekSelection> selections)
    {
        if (weekDefinition == null)
        {
            throw new ArgumentNullException(nameof(weekDefinition));
        }

        if (childState == null)
        {
            throw new ArgumentNullException(nameof(childState));
        }

        if (selections == null)
        {
            throw new ArgumentNullException(nameof(selections));
        }

        RuntimeWeekContext context = new(weekDefinition, childState);
        Dictionary<SO_CardInfoDefinition, RuntimeWeekSelection> selectionMap = BuildSelectionMap(selections);

        GameplayInteractionExecutor.ApplyAll(weekDefinition.OnWeekStartInteractions, childState);

        foreach (SO_WeekRuleDefinition weekRule in weekDefinition.WeekRules)
        {
            weekRule?.OnWeekStart(context);
        }

        foreach (WeekCardEntryData cardEntry in weekDefinition.CardEntries.OrderBy(entry => entry.DisplayOrder))
        {
            ResolveCardEntry(cardEntry, selectionMap, context);
        }

        foreach (SO_WeekRuleDefinition weekRule in weekDefinition.WeekRules)
        {
            weekRule?.OnWeekEnd(context);
        }

        GameplayInteractionExecutor.ApplyAll(weekDefinition.OnWeekEndInteractions, childState);

        return new RuntimeWeekResult(weekDefinition, context.ResolvedCards, context.WeekLogs);
    }

    private static Dictionary<SO_CardInfoDefinition, RuntimeWeekSelection> BuildSelectionMap(
        IReadOnlyList<RuntimeWeekSelection> selections)
    {
        Dictionary<SO_CardInfoDefinition, RuntimeWeekSelection> map = new();

        foreach (RuntimeWeekSelection selection in selections)
        {
            if (selection?.CardDefinition == null)
            {
                continue;
            }

            map[selection.CardDefinition] = selection;
        }

        return map;
    }

    private static void ResolveCardEntry(
        WeekCardEntryData cardEntry,
        IReadOnlyDictionary<SO_CardInfoDefinition, RuntimeWeekSelection> selectionMap,
        RuntimeWeekContext context)
    {
        if (cardEntry?.Card == null)
        {
            return;
        }

        if (!selectionMap.TryGetValue(cardEntry.Card, out RuntimeWeekSelection selection))
        {
            if (cardEntry.IsRequired)
            {
                throw new InvalidOperationException(
                    $"Required card '{cardEntry.Card.name}' is missing a runtime selection.");
            }

            return;
        }

        CardOptionData selectedOption = selection.GetSelectedOption();

        foreach (SO_WeekRuleDefinition weekRule in context.WeekDefinition.WeekRules)
        {
            weekRule?.BeforeResolveCard(context, cardEntry.Card, selectedOption);
        }

        GameplayInteractionExecutor.ApplyAll(selectedOption.Interactions, context.ChildState);

        foreach (SO_WeekRuleDefinition weekRule in context.WeekDefinition.WeekRules)
        {
            weekRule?.AfterResolveCard(context, cardEntry.Card, selectedOption);
        }

        context.AddResolvedCard(
            new RuntimeResolvedCardRecord(cardEntry.Card, selection.SelectedOptionIndex, selectedOption));
    }
}
