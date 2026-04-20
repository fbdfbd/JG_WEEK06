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

