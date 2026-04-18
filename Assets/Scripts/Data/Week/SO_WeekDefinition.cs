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
