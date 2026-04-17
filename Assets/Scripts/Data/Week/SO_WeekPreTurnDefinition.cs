using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "WeekPreTurn_",
    menuName = "Scriptable Objects/Week/WeekPreTurnDefinition")]
public class SO_WeekPreTurnDefinition : ScriptableObject
{
    [SerializeField] private string _title = string.Empty;
    [SerializeField, TextArea(2, 5)] private string _summary = string.Empty;
    [SerializeField] private WeekCardEntryData[] _informationCards = Array.Empty<WeekCardEntryData>();

    public string Title => _title;
    public string Summary => _summary;
    public WeekCardEntryData[] InformationCards => _informationCards;
}
