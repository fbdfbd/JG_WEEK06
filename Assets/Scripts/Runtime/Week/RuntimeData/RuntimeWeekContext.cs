using System.Collections.Generic;

public sealed class RuntimeWeekContext
{
    private readonly List<string> _weekLogs = new();
    private readonly List<RuntimeResolvedCardRecord> _resolvedCards = new();

    public RuntimeWeekContext(SO_WeekDefinition weekDefinition, RuntimeChildState childState)
    {
        WeekDefinition = weekDefinition;
        ChildState = childState;
    }

    public SO_WeekDefinition WeekDefinition { get; }
    public RuntimeChildState ChildState { get; }
    public IReadOnlyList<string> WeekLogs => _weekLogs;
    public IReadOnlyList<RuntimeResolvedCardRecord> ResolvedCards => _resolvedCards;

    public void ApplyInteractions(IReadOnlyList<SO_CardInteractionDefinition> interactions)
    {
        GameplayInteractionExecutor.ApplyAll(interactions, ChildState);
    }

    public void AddWeekLog(string log)
    {
        if (string.IsNullOrWhiteSpace(log))
        {
            return;
        }

        _weekLogs.Add(log);
    }

    public void AddResolvedCard(RuntimeResolvedCardRecord record)
    {
        if (record == null)
        {
            return;
        }

        _resolvedCards.Add(record);
    }
}
