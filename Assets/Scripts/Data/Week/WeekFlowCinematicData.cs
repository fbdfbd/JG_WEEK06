using System;
using DG.Tweening;
using UnityEngine;

public enum EWeekFlowCueStyle
{
    None,
    Fade,
    Slide,
    Typewriter,
    Punch,
    Custom
}

[CreateAssetMenu(fileName = "WeekFlowCue_", menuName = "Scriptable Objects/Week/WeekFlowCue")]
public class SO_WeekFlowCueDefinition : ScriptableObject
{
    [SerializeField] private string _id = string.Empty;
    [SerializeField] private EWeekFlowCueStyle _style;
    [SerializeField] private float _duration = 0.35f;
    [SerializeField] private Ease _ease = Ease.OutCubic;
    [SerializeField] private bool _blockInput = true;

    public string Id => _id;
    public EWeekFlowCueStyle Style => _style;
    public float Duration => _duration;
    public Ease Ease => _ease;
    public bool BlockInput => _blockInput;
}

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
