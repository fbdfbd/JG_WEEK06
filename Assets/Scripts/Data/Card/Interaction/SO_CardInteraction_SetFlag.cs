using UnityEngine;

[CreateAssetMenu(
    fileName = "Interaction_SetFlag",
    menuName = "Scriptable Objects/Card/Interaction/SetFlag")]
public class SO_CardInteraction_SetFlag : SO_CardInteractionDefinition
{
    [SerializeField] private EChildFlagType _flagType;

    public override void Apply(RuntimeChildState childState)
    {
        childState.SetFlag(_flagType);
    }
}
