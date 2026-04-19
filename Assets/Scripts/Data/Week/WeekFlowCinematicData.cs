using System;
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

[CreateAssetMenu(
    fileName = "WeekFlowCutsceneCatalog_",
    menuName = "Scriptable Objects/Week/WeekFlowCutsceneCatalog")]
public class SO_WeekFlowCutsceneCatalog : ScriptableObject
{
    [SerializeField] private WeekFlowCutsceneEntryData[] _entries = Array.Empty<WeekFlowCutsceneEntryData>();

    public WeekFlowCutsceneEntryData[] Entries => _entries;
}

[Serializable]
public class WeekFlowCutsceneEntryData
{
    [SerializeField] private string _displayName = string.Empty;
    [SerializeField] private EWeekFlowCutsceneMoment _moment = EWeekFlowCutsceneMoment.ScreenEnter;
    [SerializeField] private bool _useScreenType;
    [SerializeField] private EWeekFlowScreenType _screenType = EWeekFlowScreenType.EventStep;
    [SerializeField] private string _weekId = string.Empty;
    [SerializeField] private string _eventId = string.Empty;
    [SerializeField] private string _stepName = string.Empty;
    [SerializeField] private int _dialogueIndex = -1;
    [SerializeField] private string _choiceLabel = string.Empty;
    [SerializeField] private string _cutsceneId = string.Empty;

    public EWeekFlowCutsceneMoment Moment => _moment;
    public bool UseScreenType => _useScreenType;
    public EWeekFlowScreenType ScreenType => _screenType;
    public string WeekId => _weekId;
    public string EventId => _eventId;
    public string StepName => _stepName;
    public int DialogueIndex => _dialogueIndex;
    public string ChoiceLabel => _choiceLabel;
    public string CutsceneId => _cutsceneId;
}
