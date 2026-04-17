using System;
using UnityEngine;

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
