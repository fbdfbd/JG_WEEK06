using System.Collections.Generic;

public sealed class RuntimeWeekResult
{
    public RuntimeWeekResult(
        SO_WeekDefinition weekDefinition,
        IReadOnlyList<RuntimeResolvedCardRecord> resolvedCards,
        IReadOnlyList<string> weekLogs)
    {
        WeekDefinition = weekDefinition;
        ResolvedCards = resolvedCards;
        WeekLogs = weekLogs;
    }

    public SO_WeekDefinition WeekDefinition { get; }
    public IReadOnlyList<RuntimeResolvedCardRecord> ResolvedCards { get; }
    public IReadOnlyList<string> WeekLogs { get; }
}
