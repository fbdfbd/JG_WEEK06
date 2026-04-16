using System;
using UnityEngine;
using UnityEngine.Serialization;

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

    [Header("Cards")]
    [SerializeField] private WeekCardEntryData[] _cardEntries = Array.Empty<WeekCardEntryData>();

    [Header("Rules")]
    [SerializeField] private SO_WeekRuleDefinition[] _weekRules = Array.Empty<SO_WeekRuleDefinition>();

    [Header("Hooks")]
    [SerializeField] private SO_CardInteractionDefinition[] _onWeekStartInteractions = Array.Empty<SO_CardInteractionDefinition>();
    [SerializeField] private SO_CardInteractionDefinition[] _onWeekEndInteractions = Array.Empty<SO_CardInteractionDefinition>();

    [Header("Narrative")]
    [SerializeField] private WeekFixedEventData _fixedEvent;
    [SerializeField] private WeekPrivateDialogueData _privateDialogue;

    public string Id => _id;
    public int WeekIndex => _weekIndex;
    public string Title => _title;
    public string Summary => _summary;
    public WeekCardEntryData[] CardEntries => _cardEntries;
    public SO_WeekRuleDefinition[] WeekRules => _weekRules;
    public SO_CardInteractionDefinition[] OnWeekStartInteractions => _onWeekStartInteractions;
    public SO_CardInteractionDefinition[] OnWeekEndInteractions => _onWeekEndInteractions;
    public WeekFixedEventData FixedEvent => _fixedEvent;
    public WeekPrivateDialogueData PrivateDialogue => _privateDialogue;
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

[Serializable]
public class WeekFixedEventData
{
    [SerializeField] private string _title = string.Empty;
    [SerializeField, TextArea(3, 6)] private string _situationText = string.Empty;
    [SerializeField] private WeekEventReactionData[] _reactions = Array.Empty<WeekEventReactionData>();

    public string Title => _title;
    public string SituationText => _situationText;
    public WeekEventReactionData[] Reactions => _reactions;
}

[Serializable]
public class WeekEventReactionData
{
    [SerializeField] private EChildStatusType _dominantStat;
    [SerializeField, TextArea(2, 5)] private string _reactionText = string.Empty;
    [SerializeField] private SO_CardInteractionDefinition[] _interactions = Array.Empty<SO_CardInteractionDefinition>();
    [FormerlySerializedAs("_statDeltas")]
    [SerializeField, HideInInspector] private StatusDeltaData[] _legacyStatDeltas = Array.Empty<StatusDeltaData>();

    public EChildStatusType DominantStat => _dominantStat;
    public string ReactionText => _reactionText;
    public SO_CardInteractionDefinition[] Interactions => _interactions;
    public bool HasLegacyStatDeltas => _legacyStatDeltas != null && _legacyStatDeltas.Length > 0;
}

[Serializable]
public class WeekPrivateDialogueData
{
    [SerializeField] private string _title = string.Empty;
    [SerializeField, TextArea(2, 5)] private string _openingLine = string.Empty;
    [SerializeField] private DialogueChoiceData[] _choices = Array.Empty<DialogueChoiceData>();

    public string Title => _title;
    public string OpeningLine => _openingLine;
    public DialogueChoiceData[] Choices => _choices;
}

[Serializable]
public class DialogueChoiceData
{
    [SerializeField] private string _label = string.Empty;
    [SerializeField, TextArea(2, 5)] private string _responseLine = string.Empty;
    [SerializeField] private SO_CardInteractionDefinition[] _interactions = Array.Empty<SO_CardInteractionDefinition>();
    [FormerlySerializedAs("_statDeltas")]
    [SerializeField, HideInInspector] private StatusDeltaData[] _legacyStatDeltas = Array.Empty<StatusDeltaData>();

    public string Label => _label;
    public string ResponseLine => _responseLine;
    public SO_CardInteractionDefinition[] Interactions => _interactions;
    public bool HasLegacyStatDeltas => _legacyStatDeltas != null && _legacyStatDeltas.Length > 0;
}

[Serializable]
public class StatusDeltaData
{
    [SerializeField] private EChildStatusType _statType;
    [SerializeField] private int _amount;

    public EChildStatusType StatType => _statType;
    public int Amount => _amount;
}
