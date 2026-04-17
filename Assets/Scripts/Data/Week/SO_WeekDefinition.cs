using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "WeekDefinition_",
    menuName = "Scriptable Objects/Week/WeekDefinition")]
public class SO_WeekDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string _id = string.Empty;
    [SerializeField] private int _weekIndex = 1;
    [SerializeField] private string _title = string.Empty;
    [SerializeField, TextArea(2, 5)] private string _summary = string.Empty;

    [Header("Flow")]
    [SerializeField] private SO_WeekPreTurnDefinition _preTurn;
    [SerializeField] private SO_WeekDayFlowDefinition _dayFlow;
    [SerializeField] private SO_WeekNightFlowDefinition _nightFlow;

    [Header("Rules")]
    [SerializeField] private SO_WeekRuleDefinition[] _weekRules = Array.Empty<SO_WeekRuleDefinition>();

    [Header("Hooks")]
    [SerializeField] private SO_CardInteractionDefinition[] _onWeekStartInteractions = Array.Empty<SO_CardInteractionDefinition>();
    [SerializeField] private SO_CardInteractionDefinition[] _onWeekEndInteractions = Array.Empty<SO_CardInteractionDefinition>();

    [Header("Cinematics")]
    [SerializeField] private SO_WeekFlowCinematicProfile _cinematicProfile;

    public string Id => _id;
    public int WeekIndex => _weekIndex;
    public string Title => _title;
    public string Summary => _summary;
    public SO_WeekPreTurnDefinition PreTurn => _preTurn;
    public SO_WeekDayFlowDefinition DayFlow => _dayFlow;
    public SO_WeekNightFlowDefinition NightFlow => _nightFlow;
    public SO_WeekRuleDefinition[] WeekRules => _weekRules;
    public SO_CardInteractionDefinition[] OnWeekStartInteractions => _onWeekStartInteractions;
    public SO_CardInteractionDefinition[] OnWeekEndInteractions => _onWeekEndInteractions;
    public SO_WeekFlowCinematicProfile CinematicProfile => _cinematicProfile;
}

[Serializable]
public class WeekCardEntryData
{
    [SerializeField] private SO_CardInfoDefinition _card;
    [SerializeField] private bool _isRequired = true;
    [SerializeField] private int _displayOrder;

    public SO_CardInfoDefinition Card => _card;
    public bool IsRequired => _isRequired;
    public int DisplayOrder => _displayOrder;
}

[CreateAssetMenu(
    fileName = "WeekPreTurn_",
    menuName = "Scriptable Objects/Week/WeekPreTurnDefinition")]
public class SO_WeekPreTurnDefinition : ScriptableObject
{
    [SerializeField] private string _title = string.Empty;
    [SerializeField, TextArea(2, 5)] private string _summary = string.Empty;
    [SerializeField] private WeekCardEntryData[] _informationCards = Array.Empty<WeekCardEntryData>();

    public string Title => _title;
    public string Summary => _summary;
    public WeekCardEntryData[] InformationCards => _informationCards;
}

[CreateAssetMenu(
    fileName = "WeekDayFlow_",
    menuName = "Scriptable Objects/Week/WeekDayFlowDefinition")]
public class SO_WeekDayFlowDefinition : ScriptableObject
{
    [SerializeField] private SO_DayRoutineEventDefinition[] _routineEvents = Array.Empty<SO_DayRoutineEventDefinition>();
    [SerializeField] private SO_StoryEventDefinition[] _storyEvents = Array.Empty<SO_StoryEventDefinition>();

    public SO_DayRoutineEventDefinition[] RoutineEvents => _routineEvents;
    public SO_StoryEventDefinition[] StoryEvents => _storyEvents;
}

[CreateAssetMenu(
    fileName = "WeekNightFlow_",
    menuName = "Scriptable Objects/Week/WeekNightFlowDefinition")]
public class SO_WeekNightFlowDefinition : ScriptableObject
{
    [SerializeField] private SO_PrivateDialogueDefinition[] _dialogues = Array.Empty<SO_PrivateDialogueDefinition>();

    public SO_PrivateDialogueDefinition[] Dialogues => _dialogues;
}

public abstract class SO_InteractiveEventDefinition : ScriptableObject
{
    [SerializeField] private string _id = string.Empty;
    [SerializeField] private string _title = string.Empty;
    [SerializeField] private int _priority;
    [SerializeField] private WeekEventConditionData _conditions = new();
    [SerializeField] private SO_InteractiveEventStepDefinition _firstStep;
    [SerializeField] private SO_CardInteractionDefinition[] _onCompletedInteractions = Array.Empty<SO_CardInteractionDefinition>();
    [SerializeField] private SO_WeekFlowCinematicProfile _cinematicProfile;

    public string Id => _id;
    public string Title => _title;
    public int Priority => _priority;
    public WeekEventConditionData Conditions => _conditions;
    public SO_InteractiveEventStepDefinition FirstStep => _firstStep;
    public SO_CardInteractionDefinition[] OnCompletedInteractions => _onCompletedInteractions;
    public SO_WeekFlowCinematicProfile CinematicProfile => _cinematicProfile;
}

[Serializable]
public class WeekEventConditionData
{
    [SerializeField] private SO_FlagDefinition[] _requiredFlags = Array.Empty<SO_FlagDefinition>();
    [SerializeField] private SO_FlagDefinition[] _blockedFlags = Array.Empty<SO_FlagDefinition>();
    [SerializeField] private WeekStatRequirementData[] _statRequirements = Array.Empty<WeekStatRequirementData>();
    [SerializeField] private InformationTypeConditionData[] _informationRequirements = Array.Empty<InformationTypeConditionData>();

    public SO_FlagDefinition[] RequiredFlags => _requiredFlags;
    public SO_FlagDefinition[] BlockedFlags => _blockedFlags;
    public WeekStatRequirementData[] StatRequirements => _statRequirements;
    public InformationTypeConditionData[] InformationRequirements => _informationRequirements;
}

[Serializable]
public class WeekStatRequirementData
{
    [SerializeField] private EChildStatusType _statType;
    [SerializeField] private bool _useMinimum = true;
    [SerializeField] private int _minimumValue = RuntimeChildState.DefaultStatValue;
    [SerializeField] private bool _useMaximum;
    [SerializeField] private int _maximumValue = RuntimeChildState.MaxStatValue;

    public EChildStatusType StatType => _statType;
    public bool UseMinimum => _useMinimum;
    public int MinimumValue => _minimumValue;
    public bool UseMaximum => _useMaximum;
    public int MaximumValue => _maximumValue;
}

[Serializable]
public class InformationTypeConditionData
{
    [SerializeField] private SO_CardInfoTypeDefinition _informationType;
    [SerializeField] private bool _useSemanticFilter;
    [SerializeField] private ECardOptionSemantic _semantic;
    [SerializeField] private int _minimumCount = 1;

    public SO_CardInfoTypeDefinition InformationType => _informationType;
    public bool UseSemanticFilter => _useSemanticFilter;
    public ECardOptionSemantic Semantic => _semantic;
    public int MinimumCount => _minimumCount;
}

[CreateAssetMenu(
    fileName = "InteractiveEventStep_",
    menuName = "Scriptable Objects/Week/InteractiveEventStepDefinition")]
public class SO_InteractiveEventStepDefinition : ScriptableObject
{
    [SerializeField] private string _titleOverride = string.Empty;
    [SerializeField, TextArea(3, 8)] private string _bodyText = string.Empty;
    [SerializeField] private DialogueLineData[] _dialogueLines = Array.Empty<DialogueLineData>();
    [SerializeField, TextArea(2, 5)] private string _nemoLine = string.Empty;
    [SerializeField] private bool _useCustomVisualState;
    [SerializeField] private ENemoVisualState _visualState = ENemoVisualState.Neutral;
    [SerializeField] private SO_CardInteractionDefinition[] _onEnterInteractions = Array.Empty<SO_CardInteractionDefinition>();
    [SerializeField] private InteractiveEventChoiceData[] _choices = Array.Empty<InteractiveEventChoiceData>();
    [SerializeField] private SO_InteractiveEventStepDefinition _nextStep;
    [SerializeField] private WeekFlowScreenCues _cinematicCues = new();

    public string TitleOverride => _titleOverride;
    public string BodyText => _bodyText;
    public DialogueLineData[] DialogueLines => _dialogueLines;
    public string NemoLine => _nemoLine;
    public bool UseCustomVisualState => _useCustomVisualState;
    public ENemoVisualState VisualState => _visualState;
    public SO_CardInteractionDefinition[] OnEnterInteractions => _onEnterInteractions;
    public InteractiveEventChoiceData[] Choices => _choices;
    public SO_InteractiveEventStepDefinition NextStep => _nextStep;
    public WeekFlowScreenCues CinematicCues => _cinematicCues;
}

[Serializable]
public class InteractiveEventChoiceData
{
    [SerializeField] private string _label = string.Empty;
    [SerializeField] private DialogueLineData[] _responseDialogueLines = Array.Empty<DialogueLineData>();
    [SerializeField, TextArea(2, 5)] private string _responseLine = string.Empty;
    [SerializeField] private SO_CardInteractionDefinition[] _interactions = Array.Empty<SO_CardInteractionDefinition>();
    [SerializeField] private SO_InteractiveEventStepDefinition _nextStep;
    [SerializeField] private WeekFlowScreenCues _resultCinematicCues = new();

    public string Label => _label;
    public DialogueLineData[] ResponseDialogueLines => _responseDialogueLines;
    public string ResponseLine => _responseLine;
    public SO_CardInteractionDefinition[] Interactions => _interactions;
    public SO_InteractiveEventStepDefinition NextStep => _nextStep;
    public WeekFlowScreenCues ResultCinematicCues => _resultCinematicCues;
}

[CreateAssetMenu(
    fileName = "DayRoutineEvent_",
    menuName = "Scriptable Objects/Week/DayRoutineEventDefinition")]
public class SO_DayRoutineEventDefinition : SO_InteractiveEventDefinition
{
    [SerializeField] private SO_CardInfoTypeDefinition[] _relatedInformationTypes = Array.Empty<SO_CardInfoTypeDefinition>();
    [SerializeField] private ECardOptionSemantic[] _preferredSemantics = Array.Empty<ECardOptionSemantic>();

    public SO_CardInfoTypeDefinition[] RelatedInformationTypes => _relatedInformationTypes;
    public ECardOptionSemantic[] PreferredSemantics => _preferredSemantics;
}

[CreateAssetMenu(
    fileName = "StoryEvent_",
    menuName = "Scriptable Objects/Week/StoryEventDefinition")]
public class SO_StoryEventDefinition : SO_InteractiveEventDefinition
{
}

[CreateAssetMenu(
    fileName = "PrivateDialogue_",
    menuName = "Scriptable Objects/Week/PrivateDialogueDefinition")]
public class SO_PrivateDialogueDefinition : SO_InteractiveEventDefinition
{
}
