using System.Collections.Generic;
using System.Linq;
using System.Text;

public readonly struct WeekFeedbackPresentation
{
    public WeekFeedbackPresentation(
        string title,
        IReadOnlyList<string> eventLines,
        string summaryLine,
        string statDeltaLine,
        ENemoVisualState visualState)
    {
        Title = title;
        EventLines = eventLines;
        SummaryLine = summaryLine;
        StatDeltaLine = statDeltaLine;
        VisualState = visualState;
    }

    public string Title { get; }
    public IReadOnlyList<string> EventLines { get; }
    public string SummaryLine { get; }
    public string StatDeltaLine { get; }
    public ENemoVisualState VisualState { get; }
}

public static class WeekFeedbackResolver
{
    public static WeekFeedbackPresentation Resolve(
        SO_WeekDefinition executedWeek,
        RuntimeWeekResult result,
        RuntimeChildState childState,
        IReadOnlyDictionary<EChildStatusType, int> previousStats)
    {
        List<string> eventLines = result.ResolvedCards
            .Select(BuildEventLine)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Take(4)
            .ToList();

        if (eventLines.Count == 0)
        {
            eventLines.Add("이번 주엔 크게 흔들린 정보는 없었지만, 네모는 조용한 공기의 결을 오래 기억했다.");
        }

        RuntimeResolvedCardRecord lastResolvedCard = result.ResolvedCards.Count > 0
            ? result.ResolvedCards[result.ResolvedCards.Count - 1]
            : null;

        NemoFeedbackPresentation nemoFeedback = NemoFeedbackResolver.Resolve(childState, lastResolvedCard);
        string summaryLine = BuildSummaryLine(result, childState, nemoFeedback.DialogueLine);
        string statDeltaLine = BuildStatDeltaLine(childState, previousStats);
        string title = $"WEEK {executedWeek.WeekIndex} · 네모의 하루";

        return new WeekFeedbackPresentation(title, eventLines, summaryLine, statDeltaLine, nemoFeedback.VisualState);
    }

    private static string BuildEventLine(RuntimeResolvedCardRecord resolvedCard)
    {
        if (resolvedCard?.CardDefinition == null || resolvedCard.SelectedOption == null)
        {
            return string.Empty;
        }

        string title = string.IsNullOrWhiteSpace(resolvedCard.CardDefinition.Title)
            ? "이름 없는 정보"
            : resolvedCard.CardDefinition.Title;
        string optionLabel = string.IsNullOrWhiteSpace(resolvedCard.SelectedOption.Label)
            ? "처리 방식 미지정"
            : resolvedCard.SelectedOption.Label;
        string deliveredText = BuildDeliveredResultText(resolvedCard);

        return $"[{title}] {optionLabel}\n{deliveredText}";
    }

    private static string BuildDeliveredResultText(RuntimeResolvedCardRecord resolvedCard)
    {
        if (!string.IsNullOrWhiteSpace(resolvedCard.SelectedOption?.PresentedText))
        {
            return resolvedCard.SelectedOption.PresentedText;
        }

        return resolvedCard.SelectedOption?.Semantic switch
        {
            ECardOptionSemantic.Blocked => "이번 주 이 정보는 네모에게 직접 전달되지 않았다.",
            ECardOptionSemantic.Modified => "이번 주 이 정보는 다듬어진 형태로 네모에게 닿았다.",
            ECardOptionSemantic.Direct => "이번 주 이 정보는 원형에 가깝게 네모에게 전달되었다.",
            _ => "이번 주 이 정보의 전달 방식은 아직 정리되지 않았다.",
        };
    }

    private static string BuildSummaryLine(RuntimeWeekResult result, RuntimeChildState childState, string fallbackLine)
    {
        int blockedCount = result.ResolvedCards.Count(card => card.SelectedOption?.Semantic == ECardOptionSemantic.Blocked);
        int modifiedCount = result.ResolvedCards.Count(card => card.SelectedOption?.Semantic == ECardOptionSemantic.Modified);

        if (blockedCount >= 2)
        {
            return "이번 주는 숨겨진 말이 많아서, 네모는 말해지지 않은 틈을 더 오래 바라보았다.";
        }

        if (modifiedCount >= 2)
        {
            return "이번 주는 다듬어진 정보가 많아서, 네모는 들은 말의 결 사이를 스스로 메우려 했다.";
        }

        EChildStatusType dominantStat = System.Enum.GetValues(typeof(EChildStatusType))
            .Cast<EChildStatusType>()
            .OrderByDescending(statType => childState.GetStat(statType))
            .First();

        return dominantStat switch
        {
            EChildStatusType.Trust => "네모는 아직 집 안의 설명을 믿고 있지만, 그 믿음의 모양이 조금씩 달라지고 있다.",
            EChildStatusType.Curiosity => "네모는 오늘 받은 이야기들 사이에서, 아직 말해지지 않은 조각을 찾고 있다.",
            EChildStatusType.Anxiety => "네모는 무사히 하루를 지냈지만, 설명되지 않은 분위기를 오래 붙잡고 있다.",
            EChildStatusType.Obedience => "네모는 조용히 받아들이고 있지만, 그 조용함 역시 기억으로 남고 있다.",
            _ => fallbackLine,
        };
    }

    private static string BuildStatDeltaLine(
        RuntimeChildState childState,
        IReadOnlyDictionary<EChildStatusType, int> previousStats)
    {
        StringBuilder builder = new();

        AppendDelta(builder, "신뢰", childState.GetStat(EChildStatusType.Trust) - previousStats[EChildStatusType.Trust]);
        AppendDelta(builder, "호기심", childState.GetStat(EChildStatusType.Curiosity) - previousStats[EChildStatusType.Curiosity]);
        AppendDelta(builder, "불안", childState.GetStat(EChildStatusType.Anxiety) - previousStats[EChildStatusType.Anxiety]);
        AppendDelta(builder, "순응", childState.GetStat(EChildStatusType.Obedience) - previousStats[EChildStatusType.Obedience]);

        return builder.Length == 0 ? "이번 주엔 직접적인 수치 변화는 없었다." : builder.ToString().Trim();
    }

    private static void AppendDelta(StringBuilder builder, string label, int delta)
    {
        if (delta == 0)
        {
            return;
        }

        if (builder.Length > 0)
        {
            builder.Append("   ");
        }

        builder.Append(label);
        builder.Append(delta > 0 ? " +" : " ");
        builder.Append(delta);
    }
}
