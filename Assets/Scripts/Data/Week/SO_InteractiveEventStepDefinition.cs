using System;
using UnityEngine;

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
