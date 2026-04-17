using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "WeekFlowCue_", menuName = "Scriptable Objects/Week/WeekFlowCue")]
public class SO_WeekFlowCueDefinition : ScriptableObject
{
    [SerializeField] private string _id = string.Empty;
    [SerializeField] private EWeekFlowCueStyle _style;
    [SerializeField] private float _duration = 0.35f;
    [SerializeField] private Ease _ease = Ease.OutCubic;
    [SerializeField] private bool _blockInput = true;

    public string Id => _id;
    public EWeekFlowCueStyle Style => _style;
    public float Duration => _duration;
    public Ease Ease => _ease;
    public bool BlockInput => _blockInput;
}
