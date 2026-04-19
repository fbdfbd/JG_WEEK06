using UnityEngine;

[CreateAssetMenu(fileName = "SO_WeeklyTalk", menuName = "Scriptable Objects/SO_WeeklyTalk")]
public class SO_WeeklyTalk : ScriptableObject
{
    [SerializeField] private string _weekId = string.Empty;
    [SerializeField, TextArea(3, 4)] private string _context = string.Empty;
    [SerializeField] private NemoEmotionState _nemostate = NemoEmotionState.None;
    [SerializeField] private SO_CardInteraction_StatDelta _CardInteraction_StatDelta;

    public string WeekId => _weekId;
    public string Context => _context;
    public NemoEmotionState NemoState => _nemostate;

    public SO_CardInteraction_StatDelta StatDelta => _CardInteraction_StatDelta;
}

public enum NemoEmotionState
{
    Happy,
    Sad,
    Heart,
    Melancholy,
    None
}