using UnityEngine;

[CreateAssetMenu(
    fileName = "Interaction_StatDelta",
    menuName = "Scriptable Objects/Card/Interaction/StatDelta")]
public class SO_CardInteraction_StatDelta : SO_CardInteractionDefinition
{
    [SerializeField] private EChildStatusType _statType;
    [SerializeField] private int _amount;

    public override void Apply(RuntimeChildState childState)
    {
        childState.AddStat(_statType, _amount);
    }
}
