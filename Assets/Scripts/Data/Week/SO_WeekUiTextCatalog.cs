using UnityEngine;

[CreateAssetMenu(
    fileName = "WeekUiTextCatalog",
    menuName = "Scriptable Objects/Week/WeekUiTextCatalog")]
public class SO_WeekUiTextCatalog : ScriptableObject
{
    [Header("Week Header")]
    [SerializeField] private string _weekLabelFormat = "WEEK {0}";
    [SerializeField] private string _noWeekLabel = "-";
    [SerializeField] private string _noWeekTitle = "주차 없음";

    [Header("Card View")]
    [SerializeField] private string _unknownCardType = "알 수 없는 유형";

    [Header("Nemo")]
    [SerializeField] private string _defaultNemoLine = "이번 주에는 어떤 이야기가 닿을까?";

    [Header("Status Message")]
    [SerializeField] private string _endingAlreadyReachedMessage = "이미 엔딩에 도달했습니다. 다시 진행하려면 상태를 초기화하세요.";
    [SerializeField] private string _weekDefinitionMissingMessage = "WeekDefinition이 연결되지 않았습니다.";
    [SerializeField] private string _weekCompletedFormat = "WEEK {0} 실행이 완료되었습니다.";
    [SerializeField] private string _weekExecutionFailedFormat = "주 실행 중 오류가 발생했습니다: {0}";
    [SerializeField] private string _allSelectionsResetMessage = "카드 선택을 모두 초기화했습니다.";
    [SerializeField] private string _childStateResetMessage = "아이 상태를 기본값으로 초기화했습니다.";
    [SerializeField] private string _cardSelectionUpdatedMessage = "카드 선택을 변경했습니다.";
    [SerializeField] private string _weekEventAppliedMessage = "주간 고정 이벤트 효과를 적용했습니다.";
    [SerializeField] private string _privateDialogueChoiceAppliedMessage = "개인 대화 선택 결과를 적용했습니다.";
    [SerializeField] private string _endingReachedMessage = "엔딩에 도달했습니다.";
    [SerializeField] private string _movedToNextWeekFallbackMessage = "다음 주로 이동했습니다.";
    [SerializeField] private string _readyForWeekFormat = "WEEK {0} 준비가 완료되었습니다.";

    [Header("Narrative Fallback")]
    [SerializeField] private string _fixedEventTitleFallback = "주간 고정 이벤트";
    [SerializeField] private string _privateDialogueTitleFallback = "개인 대화";
    [SerializeField] private string _noEffectSummary = "효과 없음";
    [SerializeField] private string _legacyNarrativeEffectMessage = "이 내러티브 데이터에는 이전 StatDelta 데이터가 남아 있습니다. Interaction으로 마이그레이션이 필요합니다.";

    [Header("Stat Label")]
    [SerializeField] private string _trustLabel = "신뢰";
    [SerializeField] private string _curiosityLabel = "호기심";
    [SerializeField] private string _anxietyLabel = "불안";
    [SerializeField] private string _obedienceLabel = "순응";

    public string WeekLabelFormat => _weekLabelFormat;
    public string NoWeekLabel => _noWeekLabel;
    public string NoWeekTitle => _noWeekTitle;
    public string UnknownCardType => _unknownCardType;
    public string DefaultNemoLine => _defaultNemoLine;
    public string EndingAlreadyReachedMessage => _endingAlreadyReachedMessage;
    public string WeekDefinitionMissingMessage => _weekDefinitionMissingMessage;
    public string WeekCompletedFormat => _weekCompletedFormat;
    public string WeekExecutionFailedFormat => _weekExecutionFailedFormat;
    public string AllSelectionsResetMessage => _allSelectionsResetMessage;
    public string ChildStateResetMessage => _childStateResetMessage;
    public string CardSelectionUpdatedMessage => _cardSelectionUpdatedMessage;
    public string WeekEventAppliedMessage => _weekEventAppliedMessage;
    public string PrivateDialogueChoiceAppliedMessage => _privateDialogueChoiceAppliedMessage;
    public string EndingReachedMessage => _endingReachedMessage;
    public string MovedToNextWeekFallbackMessage => _movedToNextWeekFallbackMessage;
    public string ReadyForWeekFormat => _readyForWeekFormat;
    public string FixedEventTitleFallback => _fixedEventTitleFallback;
    public string PrivateDialogueTitleFallback => _privateDialogueTitleFallback;
    public string NoEffectSummary => _noEffectSummary;
    public string LegacyNarrativeEffectMessage => _legacyNarrativeEffectMessage;
    public string TrustLabel => _trustLabel;
    public string CuriosityLabel => _curiosityLabel;
    public string AnxietyLabel => _anxietyLabel;
    public string ObedienceLabel => _obedienceLabel;
}
