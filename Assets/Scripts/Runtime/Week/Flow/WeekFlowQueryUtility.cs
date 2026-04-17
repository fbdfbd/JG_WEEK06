using System;
using System.Collections.Generic;
using System.Linq;

public static class WeekFlowQueryUtility
{
    public static WeekCardEntryData[] GetCurrentWeekEntries(SO_WeekDefinition currentWeekDefinition)
    {
        if (currentWeekDefinition?.PreTurn == null || currentWeekDefinition.PreTurn.InformationCards == null)
        {
            return Array.Empty<WeekCardEntryData>();
        }

        return currentWeekDefinition.PreTurn.InformationCards
            .Where(weekCardEntry => weekCardEntry != null)
            .OrderBy(weekCardEntry => weekCardEntry.DisplayOrder)
            .ToArray();
    }

    public static Dictionary<EChildStatusType, int> CaptureCurrentStats(RuntimeChildState childState)
    {
        return new Dictionary<EChildStatusType, int>
        {
            { EChildStatusType.Trust, childState.GetStat(EChildStatusType.Trust) },
            { EChildStatusType.Curiosity, childState.GetStat(EChildStatusType.Curiosity) },
            { EChildStatusType.Anxiety, childState.GetStat(EChildStatusType.Anxiety) },
            { EChildStatusType.Obedience, childState.GetStat(EChildStatusType.Obedience) },
        };
    }
}
