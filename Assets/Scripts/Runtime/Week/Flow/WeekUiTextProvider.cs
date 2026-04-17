using System;

public sealed class WeekUiTextProvider
{
    private readonly SO_WeekUiTextCatalog _catalog;

    public WeekUiTextProvider(SO_WeekUiTextCatalog catalog)
    {
        _catalog = catalog;
    }

    public string GetWeekLabel(int weekIndex)
    {
        return string.Format(GetValue(_catalog?.WeekLabelFormat, "WEEK {0}"), weekIndex);
    }

    public string GetNoWeekLabel()
    {
        return GetValue(_catalog?.NoWeekLabel, "-");
    }

    public string GetNoWeekTitle()
    {
        return GetValue(_catalog?.NoWeekTitle, "주차 없음");
    }

    public string GetUnknownCardType()
    {
        return GetValue(_catalog?.UnknownCardType, "알 수 없는 유형");
    }

    public string GetDefaultNemoLine()
    {
        return GetValue(_catalog?.DefaultNemoLine, "이번 주에는 어떤 이야기가 닿을까?");
    }

    public string GetEndingAlreadyReachedMessage()
    {
        return GetValue(_catalog?.EndingAlreadyReachedMessage, "이미 엔딩에 도달했습니다. 다시 진행하려면 상태를 초기화하세요.");
    }

    public string GetWeekDefinitionMissingMessage()
    {
        return GetValue(_catalog?.WeekDefinitionMissingMessage, "WeekDefinition이 연결되지 않았습니다.");
    }

    public string GetWeekCompletedMessage(int weekIndex)
    {
        return string.Format(GetValue(_catalog?.WeekCompletedFormat, "WEEK {0} 실행이 완료되었습니다."), weekIndex);
    }

    public string GetWeekExecutionFailedMessage(string detail)
    {
        return string.Format(GetValue(_catalog?.WeekExecutionFailedFormat, "주 실행 중 오류가 발생했습니다: {0}"), detail);
    }

    public string GetAllSelectionsResetMessage()
    {
        return GetValue(_catalog?.AllSelectionsResetMessage, "카드 선택을 모두 초기화했습니다.");
    }

    public string GetChildStateResetMessage()
    {
        return GetValue(_catalog?.ChildStateResetMessage, "아이 상태를 기본값으로 초기화했습니다.");
    }

    public string GetCardSelectionUpdatedMessage()
    {
        return GetValue(_catalog?.CardSelectionUpdatedMessage, "카드 선택을 변경했습니다.");
    }

    public string GetWeekEventAppliedMessage()
    {
        return GetValue(_catalog?.WeekEventAppliedMessage, "주간 고정 이벤트 효과를 적용했습니다.");
    }

    public string GetPrivateDialogueChoiceAppliedMessage()
    {
        return GetValue(_catalog?.PrivateDialogueChoiceAppliedMessage, "개인 대화 선택 결과를 적용했습니다.");
    }

    public string GetEndingReachedMessage()
    {
        return GetValue(_catalog?.EndingReachedMessage, "엔딩에 도달했습니다.");
    }

    public string GetMovedToNextWeekFallbackMessage()
    {
        return GetValue(_catalog?.MovedToNextWeekFallbackMessage, "다음 주로 이동했습니다.");
    }

    public string GetReadyForWeekMessage(int weekIndex)
    {
        return string.Format(GetValue(_catalog?.ReadyForWeekFormat, "WEEK {0} 준비가 완료되었습니다."), weekIndex);
    }

    public string GetFixedEventTitleFallback()
    {
        return GetValue(_catalog?.FixedEventTitleFallback, "주간 고정 이벤트");
    }

    public string GetPrivateDialogueTitleFallback()
    {
        return GetValue(_catalog?.PrivateDialogueTitleFallback, "개인 대화");
    }

    public string GetNoEffectSummary()
    {
        return GetValue(_catalog?.NoEffectSummary, "효과 없음");
    }

    public string GetLegacyNarrativeEffectMessage()
    {
        return GetValue(_catalog?.LegacyNarrativeEffectMessage, "이 내러티브 데이터에는 이전 StatDelta 데이터가 남아 있습니다. Interaction으로 마이그레이션이 필요합니다.");
    }

    public string GetStatLabel(EChildStatusType statType)
    {
        return statType switch
        {
            EChildStatusType.Trust => GetValue(_catalog?.TrustLabel, "신뢰"),
            EChildStatusType.Curiosity => GetValue(_catalog?.CuriosityLabel, "호기심"),
            EChildStatusType.Anxiety => GetValue(_catalog?.AnxietyLabel, "불안"),
            EChildStatusType.Obedience => GetValue(_catalog?.ObedienceLabel, "순응"),
            _ => statType.ToString(),
        };
    }

    private static string GetValue(string value, string fallback)
    {
        return string.IsNullOrWhiteSpace(value) ? fallback : value;
    }
}
