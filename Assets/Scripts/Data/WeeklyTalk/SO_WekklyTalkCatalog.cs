using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_WeeklyTalkCatalog", menuName = "Scriptable Objects/SO_WeeklyTalkCatalog")]
public class SO_WeeklyTalkCatalog : ScriptableObject
{
    [SerializeField] private List<SO_WeeklyTalk> _talks = new();

    public IReadOnlyList<SO_WeeklyTalk> Talks => _talks;

    public SO_WeeklyTalk GetByWeekId(string weekId)
    {
        return _talks.Find(t => t != null && t.WeekId == weekId);
    }
}