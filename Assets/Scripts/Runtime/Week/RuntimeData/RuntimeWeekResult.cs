using System.Collections.Generic;
using System.Linq;

public sealed class RuntimeWeekResult
{
    public RuntimeWeekResult(
        SO_WeekDefinition weekDefinition,
        IReadOnlyList<RuntimeResolvedCardRecord> resolvedCards,
        IReadOnlyList<string> weekLogs,
        RuntimeInformationControlResult informationControlResult)
    {
        WeekDefinition = weekDefinition;
        ResolvedCards = resolvedCards;
        WeekLogs = weekLogs;
        InformationControlResult = informationControlResult;
    }

    public SO_WeekDefinition WeekDefinition { get; }
    public IReadOnlyList<RuntimeResolvedCardRecord> ResolvedCards { get; }
    public IReadOnlyList<string> WeekLogs { get; }
    public RuntimeInformationControlResult InformationControlResult { get; }
}

public sealed class RuntimeInformationControlResult
{
    public RuntimeInformationControlResult(
        IReadOnlyList<RuntimeInformationSelectionRecord> selections)
    {
        Selections = selections ?? System.Array.Empty<RuntimeInformationSelectionRecord>();
    }

    public IReadOnlyList<RuntimeInformationSelectionRecord> Selections { get; }

    public int CountSelectionsForType(
        SO_CardInfoTypeDefinition informationType,
        ECardOptionSemantic? semanticFilter = null)
    {
        return Selections.Count(selection =>
            selection.InformationType == informationType &&
            (!semanticFilter.HasValue || selection.Semantic == semanticFilter.Value));
    }

    public int CountSelectionsForAnyType(
        IReadOnlyList<SO_CardInfoTypeDefinition> informationTypes,
        ECardOptionSemantic? semanticFilter = null)
    {
        if (informationTypes == null || informationTypes.Count == 0)
        {
            return 0;
        }

        return Selections.Count(selection =>
            informationTypes.Contains(selection.InformationType) &&
            (!semanticFilter.HasValue || selection.Semantic == semanticFilter.Value));
    }
}

public readonly struct RuntimeInformationSelectionRecord
{
    public RuntimeInformationSelectionRecord(
        SO_CardInfoDefinition cardDefinition,
        SO_CardInfoTypeDefinition informationType,
        ECardOptionSemantic semantic)
    {
        CardDefinition = cardDefinition;
        InformationType = informationType;
        Semantic = semantic;
    }

    public SO_CardInfoDefinition CardDefinition { get; }
    public SO_CardInfoTypeDefinition InformationType { get; }
    public ECardOptionSemantic Semantic { get; }
}
