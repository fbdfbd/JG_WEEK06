using System;
using UnityEngine;
[CreateAssetMenu(
    fileName = "WeekFlowCutsceneCatalogGroup_",
    menuName = "Scriptable Objects/Week/WeekFlowCutsceneCatalogGroup")]
public class SO_WeekFlowCutsceneCatalogGroup : ScriptableObject
{
    [SerializeField] private SO_WeekFlowCutsceneCatalog _defaultCatalog;
    [SerializeField] private WeekFlowCutsceneCatalogByWeek[] _weekCatalogs = Array.Empty<WeekFlowCutsceneCatalogByWeek>();

    public SO_WeekFlowCutsceneCatalog DefaultCatalog => _defaultCatalog;
    public WeekFlowCutsceneCatalogByWeek[] WeekCatalogs => _weekCatalogs;

    public SO_WeekFlowCutsceneCatalog GetWeekCatalog(string weekId)
    {
        if (string.IsNullOrWhiteSpace(weekId) || _weekCatalogs == null)
        {
            return null;
        }

        for (int index = 0; index < _weekCatalogs.Length; index++)
        {
            WeekFlowCutsceneCatalogByWeek entry = _weekCatalogs[index];
            if (entry == null || !entry.IsMatch(weekId))
            {
                continue;
            }

            return entry.Catalog;
        }

        return null;
    }
}

[Serializable]
public class WeekFlowCutsceneCatalogByWeek
{
    [SerializeField] private string _weekId = string.Empty;
    [SerializeField] private SO_WeekFlowCutsceneCatalog _catalog;

    public string WeekId => _weekId;
    public SO_WeekFlowCutsceneCatalog Catalog => _catalog;

    public bool IsMatch(string weekId)
    {
        return !string.IsNullOrWhiteSpace(_weekId)
            && string.Equals(_weekId, weekId, StringComparison.OrdinalIgnoreCase);
    }
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

