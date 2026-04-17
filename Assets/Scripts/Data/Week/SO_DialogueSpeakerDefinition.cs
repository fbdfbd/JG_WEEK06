using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "DialogueSpeaker_",
    menuName = "Scriptable Objects/Week/DialogueSpeakerDefinition")]
public class SO_DialogueSpeakerDefinition : ScriptableObject
{
    [SerializeField] private string _id = string.Empty;
    [SerializeField] private string _displayName = string.Empty;
    [SerializeField] private ENemoVisualState _defaultVisualState = ENemoVisualState.Neutral;

    public string Id => _id;
    public string DisplayName => _displayName;
    public ENemoVisualState DefaultVisualState => _defaultVisualState;
}

[Serializable]
public class DialogueLineData
{
    [SerializeField] private SO_DialogueSpeakerDefinition _speaker;
    [SerializeField, TextArea(2, 5)] private string _text = string.Empty;

    public SO_DialogueSpeakerDefinition Speaker => _speaker;
    public string Text => _text;
}
