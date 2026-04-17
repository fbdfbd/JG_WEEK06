using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "DayRoutineEvent_",
    menuName = "Scriptable Objects/Week/DayRoutineEventDefinition")]
public class SO_DayRoutineEventDefinition : SO_InteractiveEventDefinition
{
    [SerializeField] private SO_CardInfoTypeDefinition[] _relatedInformationTypes = Array.Empty<SO_CardInfoTypeDefinition>();
    [SerializeField] private ECardOptionSemantic[] _preferredSemantics = Array.Empty<ECardOptionSemantic>();

    public SO_CardInfoTypeDefinition[] RelatedInformationTypes => _relatedInformationTypes;
    public ECardOptionSemantic[] PreferredSemantics => _preferredSemantics;
}
