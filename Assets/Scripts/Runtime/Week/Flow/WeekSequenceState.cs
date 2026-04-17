using System;
using System.Collections.Generic;
using System.Linq;

public sealed class WeekSequenceState
{
    private SO_WeekDefinition[] _orderedWeeks = Array.Empty<SO_WeekDefinition>();
    private int _currentWeekIndex;

    public SO_WeekDefinition CurrentWeekDefinition =>
        _orderedWeeks.Length == 0 ? null : _orderedWeeks[_currentWeekIndex];

    public bool IsCurrentWeekFinal =>
        _orderedWeeks.Length == 0 || _currentWeekIndex >= _orderedWeeks.Length - 1;

    public void InitializeWeekSequence(
        SO_WeekDefinition currentWeekDefinition,
        IReadOnlyList<SO_WeekDefinition> weekDefinitions)
    {
        _orderedWeeks = BuildOrderedWeeks(currentWeekDefinition, weekDefinitions);
        _currentWeekIndex = ResolveCurrentWeekIndex(currentWeekDefinition, _orderedWeeks);
    }

    public bool TryMoveToNextWeek()
    {
        if (IsCurrentWeekFinal)
        {
            return false;
        }

        _currentWeekIndex++;
        return true;
    }

    private static SO_WeekDefinition[] BuildOrderedWeeks(
        SO_WeekDefinition currentWeekDefinition,
        IReadOnlyList<SO_WeekDefinition> weekDefinitions)
    {
        List<SO_WeekDefinition> orderedWeeks = weekDefinitions?
            .Where(weekDefinition => weekDefinition != null)
            .OrderBy(weekDefinition => weekDefinition.WeekIndex)
            .ToList()
            ?? new List<SO_WeekDefinition>();

        if (orderedWeeks.Count == 0 && currentWeekDefinition != null)
        {
            orderedWeeks.Add(currentWeekDefinition);
        }

        return orderedWeeks.ToArray();
    }

    private static int ResolveCurrentWeekIndex(
        SO_WeekDefinition currentWeekDefinition,
        IReadOnlyList<SO_WeekDefinition> orderedWeeks)
    {
        if (orderedWeeks == null || orderedWeeks.Count == 0)
        {
            return 0;
        }

        int matchedIndex = currentWeekDefinition == null
            ? -1
            : Array.IndexOf(orderedWeeks.ToArray(), currentWeekDefinition);

        return matchedIndex >= 0 ? matchedIndex : 0;
    }
}
