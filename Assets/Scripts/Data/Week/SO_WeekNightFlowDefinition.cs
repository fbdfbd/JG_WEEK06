using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "WeekNightFlow_",
    menuName = "Scriptable Objects/Week/WeekNightFlowDefinition")]
public class SO_WeekNightFlowDefinition : ScriptableObject
{
    [SerializeField] private SO_PrivateDialogueDefinition[] _dialogues = Array.Empty<SO_PrivateDialogueDefinition>();

    public SO_PrivateDialogueDefinition[] Dialogues => _dialogues;
}
