using System;
using UnityEngine;

[Serializable]
public class WeekFlowScreenCues
{
    [SerializeField] private SO_WeekFlowCueDefinition _enterCue;
    [SerializeField] private SO_WeekFlowCueDefinition _exitCue;

    public SO_WeekFlowCueDefinition EnterCue => _enterCue;
    public SO_WeekFlowCueDefinition ExitCue => _exitCue;
}

[CreateAssetMenu(fileName = "WeekFlowCinematicProfile_", menuName = "Scriptable Objects/Week/WeekFlowCinematicProfile")]
public class SO_WeekFlowCinematicProfile : ScriptableObject
{
    [SerializeField] private WeekFlowScreenCues _weekFeedbackCues = new();
    [SerializeField] private WeekFlowScreenCues _eventStepCues = new();
    [SerializeField] private WeekFlowScreenCues _choiceResultCues = new();
    [SerializeField] private WeekFlowScreenCues _endingCues = new();
    [SerializeField] private SO_WeekFlowCueDefinition _weekChangeOutCue;
    [SerializeField] private SO_WeekFlowCueDefinition _weekChangeInCue;

    public WeekFlowScreenCues WeekFeedbackCues => _weekFeedbackCues;
    public WeekFlowScreenCues EventStepCues => _eventStepCues;
    public WeekFlowScreenCues ChoiceResultCues => _choiceResultCues;
    public WeekFlowScreenCues EndingCues => _endingCues;
    public SO_WeekFlowCueDefinition WeekChangeOutCue => _weekChangeOutCue;
    public SO_WeekFlowCueDefinition WeekChangeInCue => _weekChangeInCue;
}
